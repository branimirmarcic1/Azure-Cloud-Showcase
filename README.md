## **Ultimate Azure Cloud-Native Showcase**

### **Project Overview**

This project is a comprehensive showcase of designing, deploying, and operating a modern cloud-native application on the Microsoft Azure platform. It features a microservices architecture tailored to demonstrate best practices in Infrastructure as Code (IaC), CI/CD automation, container orchestration, and observability.

The system simulates a simple e-commerce backend with services for order processing and inventory management, with a strong emphasis on the software’s operational lifecycle in a cloud environment.

---

### **Technical Skills Showcase**

This project was purpose-built to demonstrate senior-level software engineering and DevOps capabilities across the entire software delivery pipeline.

* **Cloud Platform:** Microsoft Azure
* **Infrastructure as Code (IaC):** Bicep for declarative, repeatable Azure resource provisioning
* **CI/CD Automation:** GitHub Actions pipeline automates building, testing, containerization, and deployment
* **Containerization & Orchestration:** Docker for packaging services, Azure Kubernetes Service (AKS) for hosting them in a production-grade environment
* **Secure Secrets Management:** Azure Key Vault integrated via the CSI Secrets Store Driver to manage secrets securely
* **Asynchronous Communication:** RabbitMQ (locally) and MassTransit for decoupled service interactions
* **Resiliency & Fault Tolerance:** Polly for implementing retry logic in HTTP-based service communication
* **Observability & Monitoring:** OpenTelemetry for instrumentation, Prometheus for metrics collection, and Grafana for visualization

---

### **System Architecture**

The solution uses a distributed architecture with a strong focus on automation, scalability, and secure communication between services.

* **API Gateway:** In a production scenario, Azure API Management or Application Gateway would handle routing and authentication. For simplicity, this demo exposes `OrderService` directly via a LoadBalancer.
* **Service Discovery:** Microservices communicate using Kubernetes internal DNS (e.g., `http://inventoryservice-service`), eliminating dependency on IP addresses.
* **Asynchronous Messaging:** When an order is placed, `OrderService` publishes an `OrderCreated` event to RabbitMQ. Other services, like `OrderAuditorFunction`, subscribe to and react to this event independently.
* **Secrets Management:** Applications running in AKS read secrets directly from Azure Key Vault using the Secrets Store CSI driver—no secrets are stored in code or configuration.

---

### **Key Features**

* **Automated CI/CD:** Every push to the `main` branch triggers build, test, containerization, and deployment pipelines via GitHub Actions
* **Infrastructure as Code:** Complete Azure infrastructure (AKS, ACR, SQL Database, Key Vault) is provisioned using Bicep
* **Inter-Service Communication:** Supports both synchronous (HTTP) and asynchronous (event-driven via RabbitMQ) communication models
* **Centralized Monitoring:** All services are instrumented using OpenTelemetry and monitored via Prometheus and Grafana dashboards

---

### **Live Application on Azure**

The application is successfully deployed on Azure Kubernetes Service and is publicly accessible.

**Swagger UI:**
[http://4.207.237.129/swagger](http://4.207.237.129/swagger)
*Note: This public IP is temporary and may change. Instructions for retrieving the current IP are provided below.*

#### **How to Test**

1. Open the Swagger link above
2. Locate the `POST /api/Orders` endpoint
3. Click “Try it out” and input order details
4. Click “Execute” — you should receive a `200 OK` or `201 Created` response, confirming successful interaction between `OrderService` and `InventoryService` inside the AKS cluster

#### **Retrieve the Current IP Address**

To find the current external IP address of the `OrderService`:

```bash
az aks get-credentials --resource-group AzureShowcase-RG --name showcase-aks-cluster
kubectl get service orderservice-service
```

Look for the `EXTERNAL-IP` column in the output.

---

### **Running the Project Locally**

#### **Prerequisites**

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

#### **Start the Infrastructure**

From the project root directory, run:

```bash
docker-compose up -d
```

This will launch local instances of SQL Server, RabbitMQ, Prometheus, and Grafana.

#### **Run the Application**

1. Open the solution in Visual Studio
2. Configure **Multiple Startup Projects** for `OrderService`, `InventoryService`, and `OrderAuditorFunction`
3. Press **F5** to launch all services

Each service will be available on its respective `localhost` port, and you can interact with them through their Swagger UIs.
