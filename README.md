www# window-service-demo
由于公司的平板项目考虑触摸效果，想迁移到window10的系统，系统中很多原有的服务部署到window系统比较麻烦，就准备直接使用docker迁移这些服务，为了将这些docker 的镜像部署成window服务。就做了一个基于c# 的window service的demo。
要求启动和结束服务方式都是cmd命令 ，服务名，服务描述，启动服务命令， 和结束服务命令 都可以在 app.config 文件中配置

配置文件示例：

'''xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
  </startup>
  <appSettings>
    <add key="ServiceName" value="avl-rabbitmq"/>
    <add key="ServiceDescription" value="ccs 系统的后台 rabbitmq 服务，基于docker镜像建立的"/>
    <add key="StartCmd" value="docker run -d --name avl-rabbitmq -it --net host -v /e/postgres/data:/var/postgresql/data docker-registry.innovation.os/base/postgres:latest"/>
    <add key="StopCmd" value="docker stop -t 3 avl-rabbitmq &amp;&amp; docker rm avl-rabbitmq -f"/>
  </appSettings>
</configuration>
'''
