The aim of following article is to build web service using F# and ASP.NET Core using the [Giraffe library](https://github.com/dustinmoris/Giraffe). Therefore, we'll build some JSON APIs which takes data from a data source, and exposes them through HTTP. Before start, I invite you to read this article about Web development in F#. You can find the following demo on github: [https://github.com/samueleresca/Blog.FSharpOnWeb](https://github.com/samueleresca/Blog.FSharpOnWeb)

Setup the project
-----------------

Setup a Giraffe project is very simple. First of all, let's start by adding giraffe projects template: `dotnet new -i "giraffe-template::*"` The previous command install some project templates in order to build Giraffe web applications. Afterwards you can create a new Giraffe application by running `dotnet new giraffe`. The command will create a project with some files inside it. The most important is `Program.fs` file, which contains the setup of the host, the routing engine and services: https://gist.github.com/samueleresca/96acb35da592b07ac94530e13da488dc

### Project structure

As ASP.NET MVC developer, I am used to follow an naming-convention structure in order to keep separated all the components of my application: Views, Models, Startup, Routing. Giraffe simply keeps all these implementations inside the `Program.fs` file. Let's create a more structured project. In that case, I will proceed with the **following structure**:

*   `Model.fs` contains all the model which will reflect the data source structure;
*   `DataAccess.fs` defines the db context and it implements all abstractions over data manipulation in order to access to our data source;
*   RequestModels.fs implements types which define the DTOs models;
*   Handlers.fs handler accepts the `HttpHandler` type and retrieve informations from through `DataAccess.fs` functions;
*   Program.fs it is the mirror of the `Startup.cs` file into an ASP.NET Core project: it defines services, web host and routing;

### Access data through Entity framework core

EF Core is the "official" ORM shipped with ASP.NET Core. I have already talk about EF Core in the following articles: [Implementing SOLID REST API using ASP.NET Core](https://samueleresca.net/2017/02/implementing-solid-data-access-layers-using-asp-net-core/),  [Developing token authentication using ASP.NET Core](https://samueleresca.net/2016/12/developing-token-authentication-using-asp-net-core/). Obviously, EF Core is just an option, you may replace it with your favourite implementation or ORM. EF Core APIs are the same between C# and F#, therefore it will force you to fit your F# code to C# APIs. In order to proceed, we need a model which will be contained into `Model.fs` and a data context, which will be implemented into `DataAccess.fs`. Let's start by defining the model: https://gist.github.com/samueleresca/efabe6e1dae24e896b6b9fa3eb6aa82c All the models needs to expose the `[<CLIMutable>]` attribute, which allows EF Core to threat them as mutable object only @ compiler level. The `DataContext.fs` file will contain the `LabelsContext`. It will extend the base `DbContext` and it will define all the **DbSet** used by repository module: https://gist.github.com/samueleresca/cf54f0b100bbd875f7c200e5c413b0f5 Finally, we will define the `LabelRepository` module in order to perform data manipulation. Each method will return an Option<'T> type in order to handle data source exception case.

### Getting started with HttpHandler

Let's start with some fundamentals of Giraffe framework. The main building block in Giraffe is a so called `HttpHandler`: https://gist.github.com/samueleresca/b18a561be3a5bb7c3f420c26eae2814c

A `HttpHandler` is a function which takes two  arguments:  `HttpFunc`,   `HttpContext`, and it returns a `HttpContext` (wrapped in an `option` and `Task` workflow) when finished. On a high level a `HttpHandler` function receives and returns an ASP.NET Core `HttpContext` object. `HttpHandler` can process an incoming `HttpRequest` before passing it further down the Giraffe pipeline by invoking the next `HttpFunc` or short circuit the execution by returning an option of `Some HttpContext`. Each handler is mapped to an URL through the route configuration of Giraffe. Let's see concrete example inside our implementation. The following routing objects shows a mapping between some urls: `/label`, `/label/{id}` and some handlers: `labelsHandler`, `labelHandler`: The `/label` url will retrieve all the labels from database, and the `/label/%i` will retrieve a label by id. `Handler.fs` file contains implementations of handlers:

### Manage CRUD operations using HttpHandler

Handler functions are the bridge between the http client and our application. As seen before, the `Handlers.fs` file implements the `labelsHandler` methods, which retrieve informations from our datasource. Let's continue by implementing all CRUD operations in our `Handlers.fs`: `labelAddHandler` and `labelUpdateHandler` implement respectively the add operation and update operation. Each of them retrieve the `LabelContext` by using the `GetService` method. `GetService` method is part of the [dependency management system](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#dependency-management) of ASP.NET Core, which works out of the box with Giraffe. The `LabelContext` services, and in general, all services are defined into the `Program.fs` file: In that case, we are configuring a new `DbContext<LabelsContext>` by passing the connection string.

### Model validation

Giraffe also implements out-of-box a [model validation system](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#model-validation). The validation criteria can be defined into models. Indeed, in our example, we add an additional level by defining the `RequestModels.fs` file. Here is the implementation of `RequestModels.fs`: The `HasErrors` method can be used into handlers in order to check the consistency of the request and provide some feedback to the client:

Final thought
-------------

Finally, we can perform operations over data by using the following calls. This example shows you some how to build build web service using F# and ASP.NET Core, focusing on some features provided by Giraffe framework, and it is a good deep dive into web functional world for all software engineers who comes from an OOP approach. This article is not an invitation to move all your code to FP paradigm. If you think different approaches, like FP and OOP, are mutually exclusive, then I invite you to read this article: [OOP vs FP](http://blog.cleancoder.com/uncle-bob/2014/11/24/FPvsOO.html). The example is available [on github](https://github.com/samueleresca/Blog.FSharpOnWeb). Cover photo credits: [Graz - Mur Island](http://www.graz-cityofdesign.at/en/places/detail/45/mur-island)

Cheers :)