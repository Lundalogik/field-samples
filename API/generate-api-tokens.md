## How to generate API authentication tokens

In order to generate API tokens to be used instead of Lime Field passwords in public scripts you need:

1. Valid Lime Field account
2. Execute the following request, where `API_TOKEN_NAME` is a string of your choice

__Every time you execute the request the token with that name will be regenerated.__

```shell
curl -v -u YOUR_LIME_FIELD_USERNAME:YOUR_LIME_FIELD_PASSWORD -H 'Content-Type: application/json' -H 'Accept: application/json' -X PUT -d '{}' https://YOUR_LIME_FIELD_INSTALLATION_NAME.remotex.net/api/users/YOUR_LIME_FIELD_USERNAME/tokens/API_TOKEN_NAME
```

The response will have the following form:

```json
{
  "Token": "secret tokanized string"
}
```
