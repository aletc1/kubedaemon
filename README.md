# Kubedaemon
A Serverless, multi-tenant, easy-to-use, Kubernetes service launcher (jobs, work queues, GRPC/HTTP services) with scale-to-zero support.

> :warning: this project is in **early stages and under heavy development** and have not reach the alpha stage yet! So, proceed with caution.

## Motivation

How to run a batch job in Kubernetes? (A pice of code that process some data in batches and when finished, clean/frees all used resources). Kubernetes has the building blocks necesary to answer that question, but it has not a ready-to-use solution and you actually will need to architect it (And spoiler alert! it is not an easy task ☺️). And, what if I have multiple HTTP/GRPC services and I want to host them quickly and want to have a middleware that handles for me the HTTP-to-GRPC translation and offloads work using a queue-based approach? Well, that's the motivation ;)

There are some challenges/requirements we will address in this project:
- **Node selection**: There are workloads that requires GPU, others require memory-optimized VMs or can run in generic VMs. Depending on this, the job execution time will cost more or less, so we need a mechanism to first, select the propper VM, and then account the spent accordingly (useful to later pass it to customers). Beside this, the Kubernetes cluster may be running in a public cloud (Azure, Google, Amazon, Others) or on-premise, so would be nice to have a mechanism that account the spent (specially on-premises).
- **Infrastructure abstraction**: Try to hide the Kubernetes infrastructure complexity as much as possible, and provide a simple interface to handle the intended work without the fuuss. 
- **Handle parallelism**: How to divide the data in blocks and process it in parallel? How to scale pods accordingly?
- **Cost efficiency & quotas**: How to reduce costs? How to account the resources used? How to set quotas for a tenant and track storage, memory & CPU used?
- **Separation of Concerns (SoC)**: How to abstract the details of the infrastructure and the plumbing used to the end user, and just give they a simple API that "do the job" without the fuss.
- **Robustness & faul-tolerace**: How to ensure that there is not duplicated processing of the same work and ensure if there is any infrastructure fail, the workload is picked up from the last point and do not start it over again. 
- **Authorization & multi-tenancy**: How to divide the system in tenants and authorize them accordingly (e.g JWT token, RBAC authorization).
- **Observability, security, and reliability**: How to ensure (in this multi-tenant architecture) proper tenant data separation and restrict servie to service communication?
- **3rd-party system integrations**: Will be nice to integrate with other systems, and for instance, notify some of them when a job finishes (Webhooks).
- **ISV vs customer data access**: Plan what data can be seen by customers (end-users) and what can be seen by the ISVs (or the company administrators). E.g infrastructure secrets cannot be seen by customers but they can use it.
- **Event-based architecture**: Architect the solution as an Event-Sourcing, Command Query Responsibility Segregation (CQRS) system.
- **Modern Admin UI**: Every operation you can do will have an API, but would be nice to have an easy-to-use, secure, multi-tenant management UI.

There are very good projects there, like [OpenFaaS](https://www.openfaas.com/), [Hangfire](https://www.hangfire.io/), [Knative](https://cloud.google.com/knative) or [Dapr](https://dapr.io/) (just to mention a few) that somehow can be used to implement some of these requirements, but they are multi-purpose generic frameworks intended to solve different problems and not exactly the one we want to solve. For instance, OpenFaaS can scale-to-zero functions and handle queue-based workloads in parallel, but lacks of most of the previous requirements simply because has other goals.

## Goals

The kubedaemon project will follow these principles:

- **Container-centric solution**: Jobs and services will be containers, and we will not force developers to use any specific SDKs or programing languages. Every contract we define will use standards (HTTP communication, or stdin/stdout) so in theory any container can be a Job or a Service. We will define communication contracts that can be used by the container to communicate back to the solution, but always through standars. E.g Your job will only need to implement an HTTP REST API to receive queue messages (e.g HTTP POST /message). You can always debug your container using [Postman](https://www.postman.com/) or any tool that send HTTP messages.
- **True serverless**: The container will not need to implement anything to communicate with the work queues or to achieve observability. We will put automatically the required sidecards in the pod to achieve that. Under the hood we may also put a service mesh in order to ensure security and reliability.
- **Efficient resource usage**: We will work hard to minimize the requirements (storage, memory and cpu usage) of the core solution and ensure efficient ejecution management of the jobs/services in order to economize resources (scale-to-zero when needed).
- **Backend storage support**: We will implement ElasticSearch and SQL-Like databases at first. This can be extended in the future. These databases are used for event & logs storage
- **Observability & Accounting** - We will observe & log every operation and account the spent at pod level.

## Use case example

Suppose you want to build a SaaS, multi-tenant platform and your business requires that your customers can freely train some AI models to later use them inside your platform. Kubedaemon can handle the necesary plumbing for you:
- Put resource quotas to your tenants to ensure an upper-bound limit (e.g the customer cannot run a Job that requires 32GB and 8 CPU if your tenant just have paid for a limit of 1CPU and 2GB RAM).
- When running the job, tag it to select the propper Node (VM) to run the job (GPU, memory optimized or CPU optimized). Your Kubernetes can have multple node pools with different set of capacities and features. Or even, you can exploit the "Virtual Node" facility if you are in Azure Kubernetes Services.
- Automatically accounts the spent per tenant and job/service (pod level). This is very useful since public clouds accounts resource-level usage only (e.g the usage of the VM/Node as a whole), and doesnt go to tenant level (unless you divide the infrastructure per tenant but this is not practical since there might be customers that cannot afford a full infrastructure).
- Monitor the process & have logs wich you can share (or not) with the user.
- Host long running services (with scale-to-zero functionality) that can be directly called (sync) or through a queue (async)

## Roadmap

The first milestone (v0.1) may include some of the following features.

Core project
- [ ] REST API to handle everything
- [ ] Optional Admin UI (Single Page Application)
- [ ] K8s authentication & multi-cluster support
- [ ] Listen for K8s events & multi-cluster support (using [kubewatch](https://github.com/bitnami-labs/kubewatch))
- [ ] Multi-namespace support (list objects from filtered namespaces)
- [ ] Support JWT tokens and filter namespaces by signed claims (enable multi-tenancy)
- [ ] In-memory storage support (for dev/testing)
- [ ] ElasticSeach storage support
- [ ] SQL database storage support (SQL Server, PostgreSQL, MySQL, ...)
- [ ] Account tenant quotas, resources used, spent
- [ ] Store config objects (jobs, queues, etc) at Kubernetes (CustomResourceDefinition) and not in an external database

Job Launcher
- [ ] Orchestrator API (launch & manage jobs, services, view events and logs)
- [ ] Orchestrator Admin UI (launch & manage jobs, services, view events and logs)
- [ ] Launch Jobs manually in K8s (node selector labels, resource limits, volumes, input/output log, timeouts)
- [ ] Schedule jobs
- [ ] View jobs list and status
- [ ] View console log of running Job (support multiple pods logs)
- [ ] Cancel running Job
- [ ] Report progress? (0 to 100%)
- [ ] Batch jobs (Queue based workloads)

Services
- [ ] GRPC-based services
- [ ] HTTP/REST services
- [ ] GRPC to HTTP Proxy
- [ ] API Management with request/response transformation ([JSONata](https://docs.jsonata.org/overview.html)?)
- [ ] Synchronous calls (HTTP Trigger)
- [ ] Asyncrhonous calls (Queue based workloads - HTTP/Event Trigger)

Queue based workloads
- [ ] API contract for container: (HTTP /message) to post messages to container and get response (ACK, ERR, etc)
- [ ] API contract for container: (HTTP /init passing callback URL with temporary access token) Hability to produce/enqueue new messages from running workers (pods) 
- [ ] Session-based queues (one pod per session or multiple pods with multiple sessions each?)
- [ ] Control queue parallelism (max number of parallel sessions)
- [ ] Dynamically Scale up/down pods to fit sessions
- [ ] Scale to zero when no more messages in a while and revive it when a new message arrives
- [ ] Inspect queues & observability
- [ ] How to handle answer when async call is made (callback, query API or other)

Observability
- [ ] View services/jobs status (running/stopped/unallocated)
- [ ] Trace messages & exceptions
- [ ] Service map (Kiali-like)

WebHooks
- [ ] Orchestrator messages
- [ ] Application-level messages

Multi-cluster support
- [ ] Handle multiple clusters at the same time (different hosting/regions)
- [ ] A tenant can select the Cluster to run the job/service

Security
- [ ] Use a service mesh under-the-hood for security?
- [ ] Services can communicate themselves (service-to-service communication)?
- [ ] RBAC at Kubernetes level?

## Installation

TODO

## Dependencies

TODO

## License

Kubedaemon is open source and free to use under **AGPLv3** conditions. If you need a commercial license for 
If you want to use Kubedaemon for business, commercial license is also available.

## Contribute

TODO
