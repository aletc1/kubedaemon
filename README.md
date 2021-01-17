# Kubedaemon
A multi-tenant easy-to-use Kubernetes service launcher (jobs, queue workers, services) with scale-to-zero support.

> :warning: this project is under **heavy development** and have not reach the alpha stage yet!

## Motivation

TODO

## Roadmap

The first milestone (v0.1) may include some of the following features.

Core project
- [ ] REST API to handle everything
- [ ] Optional Admin UI (Single Page Application)
- [ ] K8s authentication
- [ ] Listen for K8s events (using [kubewatch](https://github.com/bitnami-labs/kubewatch))
- [ ] Multi-namespace support (list objects from filtered namespaces)
- [ ] Support JWT tokens and filter namespaces by signed claims (enable multi-tenancy)
- [ ] In-memory storage support (for dev/testing)
- [ ] ElasticSeach storage support
- [ ] SQL database storage support (SQL Server, PostgreSQL, MySQL, ...)

Job Launcher
- [ ] Orchestrator API (launch & manage jobs, view events and logs)
- [ ] Orchestrator Admin UI (launch & manage jobs, view events and logs)
- [ ] Launch Jobs manually in K8s (node selector labels, resource limits, volumes, input/output log, timeouts)
- [ ] Schedule jobs
- [ ] View jobs list and status
- [ ] View console log of running Job (support multiple pods logs)
- [ ] Cancel running Job
- [ ] Report progress? (0 to 100%)

Services
- [ ] GRPC-based services
- [ ] HTTP/REST services
- [ ] GRPC to HTTP Proxy
- [ ] Synchronous call from APIM

Queue based workloads
- [ ] API contract (HTTP) to post messages to container and get response (ACK, ERR, etc)
- [ ] API contract: Hability to produce/enqueue new messages from running workers (pods) 
- [ ] Session-based queues (one pod per session or multiple pods with multiple sessions each?)
- [ ] Control queue parallelism (max number of parallel sessions)
- [ ] Scale to zero when no more messages in a while and revive when a new message arrives
- [ ] Inspect queues & observability

Observability
- [ ] View services/jobs status (running/stopped/unallocated)
- [ ] Trace messages & exceptions
- [ ] Service map (Kiali-like)

WebHooks
- [ ] Orchestrator messages
- [ ] Application-level messages

Security
- [ ] Use a service mesh under the hood for security?

## Installation

TODO

## Dependencies

TODO

## License

Kubedaemon is open source and free to use under **AGPLv3** conditions. If you need a commercial license for 
If you want to use Kubedaemon for business, commercial license is also available.

## Contribute

TODO
