pipeline {
    agent any
    stages {
        stage('Remove Image') {
            steps {
                sh 'echo "Deploy Local Test Pipline Started..."'
                sh 'echo "1. Remove Existing Images"'
                sh 'docker rmi -f dotnet-otp:0.0'
            }
        }
        stage('Build Image') {
            steps {
                sh 'echo "2. Building Docker Image"'
                sh 'docker build -t dotnet-otp:0.0 .'
            }
        }
        stage('Run') {
            steps {
                sh 'echo "3. Run Container"'
                sh 'docker run --name otp -p 5254:5254 -dit --rm dotnet-otp:0.0 .'
            }       
        }
    }
}