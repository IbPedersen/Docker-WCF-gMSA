# Docker-WCF-gMSA
Test of WCF connectivity using gMSA in Docker, e.g. for AKS

## Test locally on same machine and same account:
Run without setting identity.

```cmd
> runServer.cmd
> runClient.cmd
```

You will in the server see, that the client was connected and some information about the connection. Depending on the UPN used, the `AuthenticationType` will be `NTLM` or `Kerberos`.

## Test with server running in Docker
### Build image
Build a Docker image with the WCF tester executable by running

```cmd
> build.cmd
```

Modify `LOCAL_VERSION` (1903) or `AKS_VERSION` (10.0.17763.737) environment varaible in `build.cmd` if other image versions is needed.

### Test without gMSA
Run without setting gMSA and identity. Use default `localhost` for client.
```cmd
> runServerInDocker.cmd
> runClient.cmd
```
The connection will fail. It will also fail,if you specify various UPNs on both server and client.

### Test with gMSA
Run without setting identity. Enter a valid file for gMSA.
```cmd
> runServerInDocker.cmd
```
The UPN used for service endpoint identity will be `User Manager\ContainerAdministrator`

Now run client, entering `User Manager\ContainerAdministrator` as endpoint identity and using default `localhost`.
```cmd
> runClient.cmd
```

The connection should succeed. The `AuthenticationType` will be `NTLM`.

Using a Kerberos identity on the endpoint does not work as the Docker using our gMSA does not have UPN/DistinguishedName.

However, using client from within Docker to a WCF server running on Windows domain will be able to use Kerberos, when specifying Kerberos identity on endpoint in both server and client.

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