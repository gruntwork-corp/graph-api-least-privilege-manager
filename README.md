# GraphApiLeastPrivilegeManager

The problem we're trying to solve with this application stem from the fact that the the existing scopes in the Microsoft Graph API for managing Groups gives too much access. \
The Groups.ReadWrite.All permission allows one to fully manage all aspects of groups (i.e. read/write/update/delete any group), hereunder create and delete groups, add and remove group members as well as administer group owners for any group. \
This is in most cases too wide a privilege for any single application in an organizational setting.

The GraphApiLeastPrivilegeManager is a .NET Core 6 Web API application that provides an interface for managing groups in  Microsoft Entra (former Azure Active Directory (AAD)) according to the [principle of least privilege](https://www.paloaltonetworks.com/cyberpedia/what-is-the-principle-of-least-privilege). \
It utilizes the Microsoft Graph API and Microsoft Entra for operations, authorization respectively. \
This application is designed following the least privilege principle and utilises application roles to ensure fine-grained access control.

## Least Privilege Implementation

In designing and implementing this application, we closely adhered to the principle of least privilege. \
This principle implies that a user should be given the minimum levels of access necessary to complete their job functions. \
We have implemented custom application roles and used Azure AD principal role assignement for access control and role-based access control (RBAC) to ensure that this principle is followed.

### Group management

With this app, we have implemented a pattern similar to the ".ReadWrite.OwnedBy" permissions as seen for some Microsoft Graph endpoints, allowing finer-grained access control over group management operations. `As a result, this app facilitates fine grained group operations by pre-authorized principals.`

In our implementation, we have the following custom roles:

- `Group.Read`: This role allows the application to get group information and list group members for authenticated principals.
- `Group.ReadWriteControl.OwnedBy`: This role allows the application to:
  - create groups.
  - update and delete groups where the authenticated principal is the owner.
- `Group.ReadWriteMember.OwnedBy`: This role allows the application to add or remove members to/from groups where the authenticated principal is the owner.
- `Group.ReadWriteOwner.OwnedBy`: This role allows the application to add or remove owners to/from groups where the authenticated principal is the owner.
- `Group.ReadWrite.OwnedBy`: This role allows the application to perform all actions listed above on groups where the authenticated principal is the owner.

When a group is created, the authenticated principal is automatically added as an owner. Only an owner can update or delete a group, add or remove members, and add or remove other owners. \
This ensures that a user or service principal can only manage the groups they own, adhering to the least privilege principle.

![preview](./docs/architecture.drawio.svg)

## Getting Started

Please follow these steps to set up and run the application.

### Prerequisites
- .NET Core 6.0
- Azure Active Directory (AAD) Tenant
- Azure App Registration with `Groups.ReadWrite.All`, `User.Read.All` & `Application.Read.All` Microsoft Graph application permissions
- Set up the required environment variables: `ARM_TENANT_ID`, `ARM_CLIENT_ID`, `ARM_CLIENT_SECRET`, `IDENTIFIER_URI`

**See [this](./Terraform/README.md) for more information about the included terraform script for bootstrapping the application & service principal you need to run this service.**\
**Beware that the the scripts also outputs the variables you need to run the service as nonsensitive variables**

### Running it locally
- Clone the repository to your local machine
- Supply the environment variables mentioned above in root level `.env`-file.
- Run the project using: `docker-compose up --build`
  - To run it with swagger: `docker-compose -f docker-compose.yaml -f docker-compose.local.yaml up --build`
  - OpenAPI documentation at: `http://localhost:5467/swagger/index.html`
- healthcheck at: `http://localhost:5467/health`
