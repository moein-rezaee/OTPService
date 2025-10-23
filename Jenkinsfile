pipeline {
    agent any
    environment {
        DOCKER_IMAGE_NAME = 'dotnet-otp:latest' // نام ایمیج Docker
        CONTAINER_NAME = 'otp' // نام کانتینر Docker
        APP_PORT = '5254' // پورت اپلیکیشن
        GIT_REPO = 'https://github.com/moein-rezaee/OTPService.git' // مسیر مخزن گیت لوکال شما
        // LOCAL_GIT_REPO = '$(pws)/git/repo' // مسیر مخزن گیت لوکال شما
        PROJECT_DIR = './src' // مسیر مخزن گیت لوکال شما
        ACTIVE_BRANCH = 'main' // مسیر مخزن گیت لوکال شما
        DEV_MODE = 'ASPNETCORE_ENVIRONMENT=Development'
    }
    stages {
        stage('Show Current Directory') {
            steps {
                sh 'echo "Current Directory:"'
                sh 'pwd'
                sh 'echo "Directory Contents:"'
                sh 'ls -la'
            }
        }
        stage('Checkout') {
            steps {
                // چک کردن کد از مخزن گیت لوکال
                sh "git clone ${GIT_REPO} || true"
                dir("${PROJECT_DIR}") {
                    script {
                        sh 'echo "Current Directory:"'
                        sh 'pwd'
                       
                        sh "git checkout ${ACTIVE_BRANCH} || true"
                        sh "git fetch --all"
                        sh "git pull origin ${ACTIVE_BRANCH}"
                       
                        sh 'echo "Directory Contents:"'
                        sh 'ls -la'
                    }
                }
            }
        }
        stage('Remove Old Image') {
            steps {
                script {
                    // پاک کردن کانتینر قبلی در صورت وجود
                    sh "docker rm -f ${CONTAINER_NAME} || true"
                    // پاک کردن ایمیج قبلی در صورت وجود
                    sh "docker rmi -f ${DOCKER_IMAGE_NAME} || true"
                }
            }
        }
        stage('Build New Image') {
            steps {
                dir("${PROJECT_DIR}") {
                    script {
                         // ساخت ایمیج جدید
                        sh "docker build -t ${DOCKER_IMAGE_NAME} ."
                    }
                }
            }
        }
        stage('Run New Container') {
            steps {
                script {
                    // ران کردن کانتینر جدید
                    sh "docker run --name ${CONTAINER_NAME} -e ${DEV_MODE} -p ${APP_PORT}:${APP_PORT} -dit --rm ${DOCKER_IMAGE_NAME}"
                }
            }
        }
    }
    post {
        always {
            // نمایش لاگ کانتینر برای بررسی
            sh "docker logs ${CONTAINER_NAME} || true"
        }
    }
}
