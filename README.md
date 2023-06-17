# Serverless Chat API

## Description
The Serverless Chat API is an AWS serverless application built using .NET 6.0 and DynamoDB. It provides a scalable and efficient chat solution by leveraging the power of AWS Lambda, API Gateway, and DynamoDB. This API works in conjunction with the open-source solution "AWS WebSocket Adapter," available at [https://github.com/fehmianac/aws-web-socket-adapter](https://github.com/fehmianac/aws-web-socket-adapter).


## Infrastructure Diagram
![infra.png](docs%2Finfra.png)


## Features
- Real-time chat functionality
- Scalable serverless architecture
- AWS Lambda for event-driven execution
- AWS API Gateway for RESTful API management
- DynamoDB for data storage
- Integration with AWS WebSocket Adapter for WebSocket communication

## Deployment
To deploy the Serverless Chat API, follow these steps:

1. Clone the GitHub repository: [https://github.com/fehmianac/serverless-chat-api](https://github.com/fehmianac/serverless-chat-api)
2. Locate the `template.yaml` file in the repository.
3. Deploy the CloudFormation stack using the `template.yaml` file. This will create the necessary AWS resources for the Serverless Chat API.

## Prerequisites
- AWS account with appropriate permissions
- .NET 6.0 SDK installed locally
- Basic knowledge of AWS services (Lambda, API Gateway, DynamoDB)
- Familiarity with serverless architecture concepts

## Getting Started
To get started with the Serverless Chat API, perform the following steps:

1. Set up an AWS account if you don't already have one.
2. Install the .NET 6.0 SDK on your development machine.
3. Clone the Serverless Chat API repository: [https://github.com/fehmianac/serverless-chat-api](https://github.com/fehmianac/serverless-chat-api)
4. Navigate to the repository's root directory.
5. Deploy the CloudFormation stack using the `template.yaml` file. This will create the necessary AWS resources.
6. Once the stack is deployed successfully, you can start using the Serverless Chat API.

## Usage
The Serverless Chat API provides the following endpoints:

![swagger.png](docs%2Fswagger.png)

## Contributing
Contributions to the Serverless Chat API are welcome! If you find any issues or want to suggest improvements, please submit an issue or pull request on the GitHub repository: [https://github.com/fehmianac/serverless-chat-api](https://github.com/fehmianac/serverless-chat-api)

## License
The Serverless Chat API is released under the [MIT License](LICENSE). Feel free to use, modify, and distribute it as per the terms of the license.

## Contact
For any questions or inquiries, please contact the project maintainer: [Fehmi Anaç](mailto:fehmianac@gmail.com)

