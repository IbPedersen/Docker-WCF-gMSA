# Docker-WCF-gMSA
Test of WCF connectivity using gMSA in Docker, e.g. for AKS.

## Test locally on same machine and same account:
Run without setting identity.
```cmd
> runServer.cmd
> runClient.cmd
```
You will in the server see, that the client was connected and some information about the connection. Depending on the UPN used, the `AuthenticationType` will be `NTLM` or `Kerberos`.

## Test locally with server running in Docker

### Build image
Build a Docker image with the WCF tester executable by running and selecting *local* 
```cmd
> build.cmd
```
Modify `LOCAL_VERSION` (1903) environment variable in `build.cmd` if other image version is needed.

### Test without gMSA
Run without setting gMSA and identity. Use default `localhost` for client.
```cmd
> runServerInDocker.cmd
> runClient.cmd
```
The connection will fail. It will also fail,if you specify various UPNs on both server and client.

### Test with gMSA using NTLM
Start server in Docker. Enter a valid json *credentialspec* file for gMSA and not setting identity.
```cmd
> runServerInDocker.cmd
```
The UPN used for service endpoint identity will be `User Manager\ContainerAdministrator`

Now run client, using default `localhost` and `User Manager\ContainerAdministrator` as endpoint identity.
```cmd
> runClient.cmd
```
The connection should succeed. The `AuthenticationType` will be `NTLM`.

### Test with gMSA and using Kerberos
Start server in Docker. Enter a valid json *credentialspec* file for gMSA and use `<domain>\<gMSA>` as identity.
```cmd
> runServerInDocker.cmd
```
Now run client, using default `localhost` and `<domain>\<gMSA>` as endpoint identity.
```cmd
> runClient.cmd
```
The connection should succeed. The `AuthenticationType` will be `Kerberos`.

## Testing in AKS
Build a Docker image with the WCF tester executable by running and selecting *AKS*
```cmd
> build.cmd
```
Modify `AKS_VERSION` (10.0.17763.737) environment variable in `build.cmd` if other image version is needed.

Use `--security-opt "credentialspec=file://<gMSAfile>"` on the pods.

### Run WCF with NTLM
Server pod:
```cmd
WCF.exe -server=60000
```
Client pod:
```cmd
WCF.exe -client=<serverHost>:60000
```

### Run WCF with Kerberos
Server pod:
```cmd
WCF.exe -server=60000 -upn=<domain>\<gMSA>
```
Client pod:
```cmd
WCF.exe -client=<serverHost>:60000 -upn=<domain>\<gMSA>
```

# Identities
On Windows there are different ways of getting the identity of the logged on user. E.g.
1. UserPrincipal.Current.UserPrincipalName: IPE@simcorp.com
2. UserPrincipal.Current.DistinguishedName: IPE@scdom.net
3. WindowsIdentity.GetCurrent().Name: SCDOM\IPE

In WCF 1 will use NTLM and 2+3 uses Kerberos.

Another account type where UPN and DistinguishedNAme are the same
1. UserPrincipal.Current.UserPrincipalName: PDMAA@scdom.net
2. UserPrincipal.Current.DistinguishedName: pdmaa@scdom.net
3. WindowsIdentity.GetCurrent().Name: SCDOM\pdmaa

WCF will use Kerberos for all these

In Docker only one of the above have value:
1. WindowsIdentity.GetCurrent().Name: User Manager\ContainerAdministrator

WCF will use NTLM when using this as endpoint identity.

However, Kerberos will be used when setting endpoint identity to the gMSA account `<domain>\<gMSA>`.