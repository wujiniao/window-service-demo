# window-service-demo
由于公司的平板项目考虑触摸效果，想迁移到window10的系统，系统中很多原有的服务部署到window系统比较麻烦，就准备直接使用docker迁移这些服务，为了将这些docker 的镜像部署成window服务。就做了一个基于c# 的window service的demo。
要求启动和结束服务方式都是cmd命令 ，服务名，服务描述，启动服务命令， 和结束服务命令 都可以在 app.config 文件中配置
