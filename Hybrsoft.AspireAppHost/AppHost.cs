using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var app = builder.AddProject<Hybrsoft_FoundationAPI>("hybrsoftfoundationapi");

builder.Build().Run();
