#### 一、使用c#写一个日志转换程序，能够将文本格式的日志文件解析并转换成可读性更好的html格式文件。

日志格式为:
- [level.tag date time] content

中括号中为日志头内容
- level 表示日志等级，包含：Debug,Info,Warn,Error 4个等级
- tag 为日志标签，和level之间用.分隔，可选项
- date time 为日志时间
- content 为日志具体内容

要求：
- 程序的命令形式为：logconv inputfile outputfile
- 输出的html格式请参考output.html
- 对日志中可能存在的特殊字符需要在html中正确显示
- 能够对日志内容中存在的多行文本进行正确的换行处理

#### 二、...