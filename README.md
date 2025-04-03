# HyperCrawlX

## 🚀 About the Project
**HyperCrawlX** is a distributed web crawler designed to efficiently discover product URLs on e-commerce websites.<br>
Leveraging modern web scraping technologies, it enables seamless extraction of e-commerce data.

## 🔥 Features
- Distributed crawling for scalable and efficient data discovery
- **Asynchronous Processing** with multiple workers
- High-performance scraping using **Playwright** and **HtmlAgilityPack**
- **PostgreSQL** for structured data storage with **Connection Pooling** for efficient DB connection management.
- Containerized using **Docker**
- Cloud-based deployment on **AWS ECS** and **Render**

## High-Level Design
![Architecture](https://github.com/user-attachments/assets/c62bb74f-a2fe-416b-a240-96fbbd8c39eb)

## 🛠️ Tech Stack
- **Backend:** .NET Core, C#
- **Database:** PostgreSQL
- **Scraping:** Playwright, HtmlAgilityPack
- **Deployment:** Docker, AWS ECS, Render

## 📌 Usage
HyperCrawlX operates as a web application. Users can interact with the crawler via its API endpoints.

### 1️⃣ Submit a Crawl Request
```http
POST http://hypercrawlx-lb-1875153021.ap-south-1.elb.amazonaws.com/hypercrawlx/submitCrawlRequest
Content-Type: application/json
{
  "url": "<E-com website URL>"
}
```
#### ✅ Response
```json
{
  "requestId": "<unique Id of the request>",
  "url": "<E-com website URL>"
}
```
This API submits a crawl request to the database with the given URL.
The URL is validated for structural correctness before submitting the request. If found altered, it is rejected and appropriate error is thrown to the user.

### 2️⃣ Check Crawl Status
```http
GET http://hypercrawlx-lb-1875153021.ap-south-1.elb.amazonaws.com/hypercrawlx/getRequestStatus/<request-Id>
```
#### ✅ Response
```json
{
  "requestId": "<unique id of the request>",
  "status": "Completed",
  "url": "<E-com website URL>",
  "productUrlsCount": "<count-of-the-urls-found>",
  "productUrls": [
    "https://example.com/product1",
    "https://example.com/product2"
  ]
}
```
This API is used to check the status of the request. The status can be one of the following:
1. Queued
2. InProgress
3. Completed
4. Failed

## 🐳 Docker Image
Here is the docker image tag for this app
`anu1201d/apps-pub-repo:hypercrawlx`

---

💡 *HyperCrawlX - Simplifying e-commerce product discovery at scale!*
