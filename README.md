# Fun with Stack

Just for fun tool to look at your call stack content in runtime

*current version works only for 32bit*

There are two tools to examine call stack ```StackViewer``` and ```StackWalker```

To print contents of all stack frames use:
```csharp
StackViewer.ShowStack();
```

To walk on stack byte-by-byte use:
```csharp
StackWalker.Walk();
```
To walk use ```Up``` and ```Down``` keys. To exit use ```q``` key.
