# AWS Deployment Guide for Mini API

This project demonstrates how to build, push, and deploy a Dockerized application using **Amazon ECR (Elastic Container Registry)**, with options to run on **ECS, EKS, or App Runner**.

---

## Steps

### 1. Create an ECR Repository
```bash
aws ecr create-repository --repository-name ecr-demo --region ap-southeast-2
```

---

### 2. Authenticate Docker to ECR
```bash
aws ecr get-login-password --region ap-southeast-2 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.ap-southeast-2.amazonaws.com
```

---

### 3. Build Your Docker Image
```bash
docker build -t ecr-mini-api:latest .
```

---

### 4. Tag the Image for ECR
```bash
docker tag ecr-mini-api:latest <account-id>.dkr.ecr.ap-southeast-2.amazonaws.com/ecr-demo:latest
```

---

### 5. Push the Image to ECR
```bash
docker push <account-id>.dkr.ecr.ap-southeast-2.amazonaws.com/ecr-demo:latest
```

---

### 6. Sanity Check (Optional)
Pull the image back to confirm the push worked:
```bash
docker pull <account-id>.dkr.ecr.ap-southeast-2.amazonaws.com/ecr-demo:latest
```

---

### 7. Deploy from ECR

Once the image is in ECR, it can be deployed to:
- **ECS (Fargate/EC2)** → via task definitions  
- **EKS (Kubernetes)** → specify in your Deployment YAML  
- **App Runner** → directly reference the ECR image  

#### 7.1 Kubernetes Example
```yaml
containers:
  - name: k8s-miniapi-demo
    image: <account-id>.dkr.ecr.ap-southeast-2.amazonaws.com/ecr-demo:latest
```

#### 7.2 App Runner Quick Deployment

##### 7.2.1 Create an IAM Role for App Runner
```bash
aws iam create-role --role-name AppRunnerECRAccessRole --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Principal": {
          "Service": "build.apprunner.amazonaws.com"
        },
        "Action": "sts:AssumeRole"
      }
    ]
  }'
```

##### 7.2.2 Attach the Managed Policy for ECR Access
```bash
aws iam attach-role-policy --role-name AppRunnerECRAccessRole --policy-arn arn:aws:iam::aws:policy/service-role/AWSAppRunnerServicePolicyForECRAccess
```

##### 7.2.3 Deploy with the Role
```bash
aws apprunner create-service --service-name mini-api-demo --source-configuration '{
    "ImageRepository": {
      "ImageIdentifier": "<account-id>.dkr.ecr.ap-southeast-2.amazonaws.com/ecr-demo:latest",
      "ImageRepositoryType": "ECR",
      "ImageConfiguration": { "Port": "8080" }
    },
    "AuthenticationConfiguration": {
      "AccessRoleArn": "arn:aws:iam::<account-id>:role/AppRunnerECRAccessRole"
    }
  }'
```

---

## Summary
- Build and tag Docker images.  
- Push to ECR.  
- Deploy using ECS, EKS, or App Runner.  

Your image is now fully ready for cloud deployment 🚀

---

## Updating the Deployment

When modifying the code and wanting to deploy again:
- Build the Docker Image again  
- Tag the Image for ECR
- Authenticate Docker to ECR
- Push the Image to ECR  
- Trigger App Runner to redeploy  

```bash
aws apprunner start-deployment --service-arn arn:aws:apprunner:ap-southeast-2:<account-id>:service/mini-api-demo/<arn-string>
```
