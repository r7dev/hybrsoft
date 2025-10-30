using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Hybrsoft_FoundationAPI>("hybrsoftfoundationapi");

builder.Build().Run();
