## High-Performance Data Modeling

### The Scenario

Our system handles thousands of companies. We have moved away from Relational databases to DynamoDB to handle massive scale. However, we need to satisfy a complex set of business requirements using Single Table Design.

You must design a schema for a single DynamoDB table that supports the following access patterns efficiently (using Query, never Scan).

### Main Query

Given a CompanyID, fetch the current Company metadata, the full history of metadata changes (versions), and all Users belonging to that company, all in one single network request.

### Complications

- Users can log in using their email. We need to find a User record by their Email across the entire system (all companies combined) instantly.
- Some companies have 50,000 users. How do you ensure main query doesn't fail or time out due to the 1MB response limit in DynamoDB?

---

### Tasks

#### 1. Schema Design

Propose the Partition Key (PK) and Sort Key (SK) values for the following entities:

- **Company Metadata (Current)**: What are the PK/SK?
- **Company Version History (Audit Trail)**: How do you store previous versions?
- **User Records**: What are the PK/SK?

#### 2. Versioning

If an admin updates the company name, we need to preserve the old name for legal audit.

- How do you store the v1, v2, v3 records?
- When performing main query, how do you ensure the "Current/Latest" version is easy to identify among the history?

#### 3. Global Secondary Index (GSI)

- Explain how you would design a GSI to satisfy lookup by email complication.
- What would be the PK and SK of this index?

#### 4. Implementation

- Describe the parameters for your Query call for main query.
- How do you use the Sort Key to ensure Users and Metadata come back together?
- How would you handle Pagination if a company has thousands of users?
- Describe the KeyConditionExpression and ExpressionAttributeValues required to pull the "Item Collection."
- You may use the standard AWS SDK or high-performance alternatives (e.g., EfficientDynamoDB). Regardless of the tool, explain how you leverage the Partition Key and Sort Key to avoid multiple round-trips to the database.

---

#### Report:

1. Schema Design

Single Table Design, the idea is that if you give every entity the same PK format (COMPANY#id), DynamoDB physically stores them together,
which means one Query call is enough to pull everything for a company at once.

| Entity            | PK             | SK                     |
|-------------------|----------------|------------------------|
| Company (current) | COMPANY#123    | #METADATA              |
| Company version   | COMPANY#123    | #VERSION#2024-01-15    |
| User              | COMPANY#123    | USER#abc123            |

https://www.alexdebrie.com/posts/dynamodb-single-table/

2. Versioning

When an admin updates the company name, the process is:
1. Take the current #METADATA item and write a copy of it with SK set to #VERSION#\<timestamp\>
2. Then update the #METADATA item with the new values

This way the current state is always at SK = #METADATA, which is easy to identify without any filtering.
All historical versions are under #VERSION# and DynamoDB sorts them chronologically since timestamps sort naturally as strings.

https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-sort-keys.html

3. GSI for Email Lookup

A Global Secondary Index with Email as the partition key handles this cleanly. Since GSIs project attributes from the base table
each user item that has an Email field will automatically appear in the index.
A login query against the index only needs the email address and comes back instantly regardless of which company the user belongs to.
GSI-PK: Email, GSI-SK: not needed (email is unique enough)

https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/GSI.html

4. Implementation

For the main query, the KeyConditionExpression is just a match on PK. Since all entity types share the same PK,
a single call returns company metadata, version history, and all users together:

var request = new QueryRequest
{
    TableName = "MyTable",
    KeyConditionExpression = "PK = :pk",
    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
    {
        { ":pk", new AttributeValue { S = "COMPANY#123" } }
    }
};

The Sort Key is what keeps things organized on the way back, metadata comes first (#METADATA sorts before #VERSION and USER alphabetically), then version history, then users.

5. Pagination

Since DynamoDB caps each response at 1MB. For large companies this means the response will be cut off and a LastEvaluatedKey 
will be returned as a continuation token. You just keep calling with that token as ExclusiveStartKey until it comes back empty:
do
{
    var response = await client.QueryAsync(request);
    items.AddRange(response.Items);
    request.ExclusiveStartKey = response.LastEvaluatedKey;
}
while (response.LastEvaluatedKey?.Count > 0);

For a UI use case where you don't need all 50,000 users at once,
you would add a Limit to the query and only fetch the next page when the user asks for it.

https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Query.Pagination.html