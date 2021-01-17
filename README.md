# Kubedaemon
A multi-tenant easy-to-use Kubernetes service launcher (jobs, queue workers, services) with scale-to-zero support.

> :warning: this project is under **heavy development** and have not reach the alpha stage yet!

## Motivation

TODO


## Roadmap

Core project
- [ ] Listen for K8s events (using [kubewatch](https://github.com/bitnami-labs/kubewatch))
- [ ] Multi-namespace support (list objects from filtered namespaces)
- [ ] In-memory storage
- [ ] ElasticSeach storage

Job Launcher
- [ ] Orchestrator API (launch & manage jobs, view events and logs) 
- [ ] Launch Jobs manually in K8s (node selector labels, resource limits, volumes, input/output log, timeouts)
- [ ] Schedule jobs
- [ ] View jobs list and status
- [ ] View console log of running Job (support multiple pods logs)
- [ ] Cancel running Job
- [ ] Open a pull request

Queue based workloads
- [ ] API contract (HTTP) to post messages to container and response (ACK, ERR, etc)
- [ ] Session-based queues (one pod per session?)
- [ ] Control queue parallelism (max parallel sessions)
- [ ] Scale to zero

Services
- [ ] GRPC-based services
- [ ] HTTP/REST services
- [ ] GRPC to HTTP Proxy

Observability

WebHooks
- [ ] Orchestrator messages
- [ ] Application-level messages

## Installation

TODO

## Dependencies

TODO

## License

Kubedaemon is open source and free to use under **AGPLv3** conditions. If you need a commercial license for 
If you want to use Kubedaemon for business, commercial license is also available.

## Contribute

TODO
