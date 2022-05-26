from apiClient import * 
import json

print('Example: calculating auth token with a dummy' )
token = sophtron_auth_code(os.environ['SophtronApiUserId'], os.environ['SophtronApiUserSecret'], 'exapmple/', 'GET')
print(token)

print('Retriving an example institution "Sophtron Bank"' )
client = SophtronApiClient('https://api.sophtron.com/api/', os.environ['SophtronApiUserId'], os.environ['SophtronApiUserSecret'])

banks = client.getInstitutionByName('Sophtron Bank')

print(banks[0]['InstitutionName'])
