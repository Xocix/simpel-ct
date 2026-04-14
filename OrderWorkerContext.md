## Order Worker Context

You have been assigned to investigate a data integrity issue in our Order Processing Service. This is a C# .NET 10 background worker that consumes messages from an SQS queue and interacts with our RDS database.

We have provided a simplified version of the production code. While it uses an InMemory database and a MockQueue, you should treat the logic as if it were interacting with actual AWS services.

### Incident Report: "The Price Mismatch"

We have a major issue with Order #1001. An administrator updated the Price of this order directly in the system to correct a data entry error.
However, even after the change was confirmed in the database, the background worker processed the order using the original, incorrect price.

We need to know why the worker ignored the updated database record and how to ensure this never happens again.

Analyze the provided solution, specifically OrderWorker.cs.

Find the Root Cause: Why did the worker use the old price even though the database was updated externally?

**The "What Else":** While investigating the price bug, did you notice any other technical issues or inefficiencies that would impact production stability or costs?

### What we expect from your solution:

- **Reproduce the Issue**: Ensure you can reproduce the price mismatch issue locally using the provided code.
- **Analyze the Code**: Carefully review OrderWorker.cs to understand how it interacts with the database and the message queue.
- **Identify the Root Cause**: Determine why the worker processed the order with the old price despite the database update.
- **Propose a Solution**: Suggest a fix to ensure the worker always uses the latest price from the database.
- **Document Findings**: Write a detailed report explaining your findings, the root cause, and the proposed solution.

#### Bonus: Brief written answers to the Cloud Architecture questions below

- **Bonus 1**: If we scale to 500+ instances of this worker, how do we protect our RDS database from connection exhaustion?
- **Bonus 2**: If a message fails to process due to an error, how should we handle it to prevent it from blocking the rest of the queue?
- **Bonus 3**: How do we ensure a customer isn't charged twice if S3/SQS delivers the same message more than once?

#### Report:
The DbContext was registered as a Singleton, causing it to cache entities. When FindAsync() is called,
it returns the cached entity instead of querying the database(if it's cached, if not it will query database),
so external updates were never visible to the worker.
Fix: Use IDbContextFactory to create a fresh context per message, ensuring always fresh data from DB.

Other Issue: Original code had no delay when queue was empty causing 100% CPU.
Fix: Added Task.Delay(500) when no message is received.

Bonus 1: if we scale to 500+ workers, We could use Amazon RDS Proxy https://aws.amazon.com/blogs/database/scaling-your-amazon-rds-instance-vertically-and-horizontally/
Bonus 2: Message fails, we could configure SQS Dead Letter Queue (DLQ) https://www.conduktor.io/glossary/dead-letter-queues-for-error-handling
Bonus 3: implement idempotency by checking MessageId in a "processed" table before processing https://microservices.io/post/microservices/patterns/2020/10/16/idempotent-consumer.html#:~:text=You%20must%20use%20the%20Idempotent%20Consumer%20pattern,problems%20by%20making%20your%20message%20handlers%20idempotent.
or use SQS FIFO queue with deduplication enabled: https://aws.amazon.com/sqs/faqs/#fifo-queues--1kknyrp