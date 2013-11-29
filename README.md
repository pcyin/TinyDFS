TinyDFS
=======

a tiny distributed file system

1	项目简介

TinyDFS是殷鹏程作为分布式系统大作业，所开发的一款简单的Key/Value式分布式文件系统，整个项目基于C#+Windows Communication Foundation，实现了一个分布式文件系统所必须的上传与下载文件的功能。


2	系统架构

2.1	系统构成

TinyDFS由三大部分构成：客户端（TinyDFS.Client），命名解析主节点（TinyDFS.NameNode），文件块服务器（TinyDFS.FileServer）。不同的组件之前通过WCF，基于标准的HTTP协议进行通信。因此可以分散到不同的物理主机上执行。

2.1.1	客户端

客户端是一个简单的控制台程序。可以进行上传文件、下载文件、删除文件、列出文件列表等命令。它通过标准的HTTP协议与NameNode和块服务器通信，完成相应的操作。

2.1.2	命名解析主节点

NameNode用于进行整个分布式文件系统的名字解析。它维护着文件名与文件的所有块之间的映射信息。

TinyDFS中，文件存储的最小单位是块（Chunk）。对于大小超过64MB的文件，其被分割为若干大小在64MB左右的块，分散地存储在不同的块服务器中。每个块由一个128Bit的GUID来唯一标识。NameNode维护了每个文件名与其对应的块的映射。

TinyDFS中没有目录的概念，文件按名（Key）存取，名是一个任意长度的非空字符串，当然也可以将一个文件命名为类似于“filedirA/filedirB/filename”这样的形式。

2.1.3	块服务器

块服务器负责具体的文件块的存放。块在块服务器上以对应的GUID命名。块服务器为客户端请求块下载块内容提供了标准的WCF接口。
