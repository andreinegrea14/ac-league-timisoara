# Visma - LigaAC Labs - Application Security

# Transport layer protection

## Packages needed to regenerate the identity models 
---
Scaffolding commands powershell

To recreate the identity database schema (the project already has it hence this is not needed to be run again):

```
Add-Migration CreateIdentitySchema -Context InjectionContext
```

To generate the identity database:

```
Update-Database -Context InjectionContext
```

## Documentation
https://docs.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-3.1&tabs=netcore-cli#scaffold-identity-into-an-mvc-project-without-existing-authorization

# Disclosure
None of the solutions gathered here are production ready! 

One should treat them with care and not deploy to public web sites.


