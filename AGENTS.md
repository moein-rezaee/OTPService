Agent Guide (AI‑Facing)

Purpose and Scope
- This single document is the authoritative, root‑level guide for any AI building or evolving code in this repository.
- It defines complete, opinionated checklists for:
  - Shared extensions (libraries) used by multiple services
  - New microservices (from scratch) with consistent architecture and conventions
- Treat all items as hard requirements unless explicitly stated as optional.

Global Conventions (Applies to Everything)
- Repo layout: src/ (services) and shared/ (libraries)
- Target Framework: net9.0 (use multi‑target net8.0 if consumers require)
- Microsoft.Extensions.* packages: prefer 9.0.0 to match TFM
- Clean code: no commented/dead code, no XML Swagger comments in controllers or libraries
- Namespaces: use using statements; do not inline fully‑qualified namespaces
- Routing: only the prefix segment must be lowercase "api"; other segments remain PascalCase
  - Example: /api/Otp/Send, /api/Otp/Verify
- Swagger: centralized under <Service>.Api/Swagger; Program.cs only wires AddSwaggerGen and UseSwagger/UI
- Configuration: support ${ENV_VAR} placeholders resolved by extensions; app loads .env via DotNetEnv
- Security: never log secrets; produce concise, non‑sensitive error messages

Extensions (Libraries) — Checklist and Rules

Foldering and Naming
- Location: shared/<ExtensionName>
  - Example: shared/SmsProvider, shared/CacheService
- Namespaces (RootNamespace in csproj):
  - SmsExtension.* for SMS, CacheExtension.* for Cache, etc.
- Subfolders (required where applicable):
  - Abstractions: public interfaces (e.g., ISmsProvider, ISmsProviderFactory, ICacheService, ICacheServiceFactory)
  - Core: Options (DefaultProviderOptions), factories (SmsProviderFactory/CacheServiceFactory), default decorators (DefaultSmsProvider), helpers
  - Providers (or Provider.*): concrete implementations per external system
  - DependencyInjection.cs: IServiceCollection extension entrypoint

Design and DI
- Single entrypoint per extension:
  - public static IServiceCollection Add<ExtensionName>Extension(this IServiceCollection services, IConfiguration configuration)
  - All wiring, option binding, env placeholder expansion, and validation happen inside this method
- Factories and defaults:
  - Provide DefaultProviderOptions with string DefaultProvider (e.g., "Kavenegar"|"Farapayamak" or "Memory"|"Redis")
  - Provide Factory with Get(kind), GetDefault(), GetAll()
  - Provide DefaultXxxProvider that delegates to the selected concrete provider
- Register only configured providers (validate minimum required options)
  - SMS
    - Kavenegar requires ApiKey + Sender
    - Farapayamak requires Username + Password + From (map Sender→From if only Sender provided)
  - Cache
    - Redis requires Host + Port (Username/Password optional)
- Fallbacks and errors:
  - Choose default provider from configuration; throw a clear exception if no usable provider is configured

Configuration Contracts (required)
- SMS
  - Sms:DefaultProvider or Sms:Provider: "Kavenegar" | "Farapayamak"
  - Sms:Providers:Kavenegar: ApiKey, Sender
  - Sms:Providers:Farapayamak: Username, Password, From (Sender may map to From)
- Cache
  - Cache:DefaultProvider: "Memory" | "Redis"
  - Cache:Providers:Redis: Host, Port, Username?, Password?, InstanceName?, DefaultDatabase?
- Environment placeholders supported: ${ENV_VAR} (expand inside Add<ExtensionName>Extension before Configure<TOptions>)

Clean Code and SOLID (extensions)
- SRP: each provider integrates one external system only
- OCP: add new providers by adding classes/registrations; avoid modifying existing behavior
- DIP: consumers depend on abstractions (ISmsProvider/ICacheService), not concretes
- Nullability enabled; implicit usings enabled; no commented code

Logging and Errors (extensions)
- Use ILogger in providers and factories (Debug on invocation; Error on failures)
- Do not log secrets; return concise error messages
- For sync SDKs, use Task.Run sparingly with cancellation tokens

Testing and Packaging (extensions)
- Provide a fake/in‑memory provider for dev/test when feasible
- Optional: GeneratePackageOnBuild + package metadata for internal registry publishing

Extensions — Acceptance Checklist (AI must verify)
- [] Folder at shared/<ExtensionName> created with Abstractions/Core/Providers/DependencyInjection.cs
- [] net9.0 TFM and Microsoft.Extensions.* 9.0.0 in csproj
- [] DefaultProviderOptions + Factory + Default provider implemented
- [] Only validated providers registered; default selection implemented
- [] Add<ExtensionName>Extension expands ${ENV} and binds options
- [] Logging in providers/factory; no secrets in logs
- [] Sample consumer compiles and runs with this extension


Microservices — Checklist and Rules

Repository Layout per service
- src/<Service>.Api (Web API)
- src/<Service>.Application (CQRS, handlers, validators, app services)
- src/<Service>.Domain (interfaces/contracts)
- src/<Service>.Infrastructure (infra implementations if needed; cross‑cutting covered by shared)

Program.cs (Api)
- DotNetEnv.Env.Load() at start
- AddControllers(), AddEndpointsApiExplorer()
- AddSwaggerGen(options => options.ConfigureSwagger());
- builder.Services.AddCacheExtension(builder.Configuration)
- builder.Services.AddSmsExtension(builder.Configuration)
- MediatR: RegisterServicesFromAssembly(typeof(AnyRequestFromApplication).Assembly)
- FluentValidation: AddValidatorsFromAssemblyContaining<AnyValidator>() + AddFluentValidationAutoValidation()
- CORS default (loosen or tighten per service)
- app.UseSwagger(); app.UseSwaggerUI(…)
- app.UseHttpsRedirection(); app.UseCors(); app.UseAuthorization(); app.MapControllers()

Routing
- Prefix: lowercase "api" only; other segments are PascalCase
- Controllers: [Route("api/[controller]")]
- Actions: PascalCase (e.g., [HttpPost("Send")])
- Do NOT enable global LowercaseUrls

Swagger
- Centralized under src/<Service>.Api/Swagger
  - SwaggerConfiguration.cs: ConfigureSwagger
  - SwaggerDefaultValues.cs: operation filter with null‑safe defaults
- No XML summary/response comments in controllers

Application/Domain/Infrastructure separation
- Application: CQRS handlers thin; validators near DTOs; orchestration only
- Domain: pure contracts/interfaces; no infra dependencies
- Infrastructure: IO/DB integrations if not already in shared

Using shared extensions
- Always use extension methods in Program.cs (AddCacheExtension/AddSmsExtension)
- Default services:
  - ISmsProvider → DefaultSmsProvider (selection via config)
  - ISmsProviderFactory for runtime selection (Get/GetDefault/GetAll)
  - ICacheService → default selection; ICacheServiceFactory available

Service Configuration Contracts
- Cache: DefaultProvider; Providers:Redis (Host, Port, Username?, Password?, …)
- SMS: DefaultProvider or Provider; Providers:Kavenegar (ApiKey, Sender); Providers:Farapayamak (Username, Password, From)
- ${ENV_VAR} placeholders supported and expanded in the shared extensions

Clean Code and SOLID (services)
- SRP/OCP/DIP enforced; controllers thin; no DI/config logic outside Program.cs and shared extensions
- No fully‑qualified namespaces inline; always use using
- Nullability enabled; no commented code

Docker and Compose
- Dockerfile: sdk:9.0 and aspnet:9.0; restore via copying solution + csproj (src/* and shared/*), then copy source and publish
- docker‑compose.yml: mount appsettings.json and .env from src/<Service>.Api; expose ports and optional Redis

Microservice — Acceptance Checklist (AI must verify)
- [] Four projects created under src/<Service>.* with proper references
- [] Program.cs minimal and calls shared extensions
- [] Routing conforms: /api/<Controller>/<Action> with correct casing
- [] Swagger centralized; Program wires it only
- [] Appsettings + .env align with contracts; ${ENV} works via shared extensions
- [] Build and run succeeds; Swagger is reachable


AI Routine: Canonical Steps and Prompts
- When creating a new extension
  - Ask: Extension name? Required providers? Default provider? Config keys available? Any env names?
  - Create shared/<ExtensionName>/Abstractions|Core|Providers|DependencyInjection.cs
  - Implement Options, Factory, Default provider, concrete providers; bind options with ${ENV} expansion
  - Add csproj (net9.0; Microsoft.Extensions.* 9.0.0) and verify build

- When creating a new microservice
  - Ask: Service name? Endpoints? Default SMS/Cache providers? Any external dependencies?
  - Create 4 projects under src/<Service>.* and wire references
  - Add Swagger folder and Program.cs minimal wiring
  - Add controllers with [Route("api/[controller]")] and PascalCase actions
  - Wire AddCacheExtension/AddSmsExtension; validate configuration and run

Don’ts for Agents
- Don’t wire providers directly in Program.cs; always use Add<ExtensionName>Extension
- Don’t enable global lowercase URLs; only the "api" prefix is lowercase
- Don’t add XML summary comments or commented code
- Don’t log sensitive values or hardcode secrets

