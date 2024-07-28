mport http.client 
import json 
import argparse 
 
def main(amount): 
    # Convert the amount to string if needed for the payload 
    amount_str = str(amount) 
     
    conn = http.client.HTTPSConnection("service.maschain.com") 
    payload = json.dumps({ 
        "wallet_address": "0x7Bce030BeFAa4Feb0c16808Be9E8b273d47CdD58", 
        "to": "0x0Ac0B17998b7e5b2bbef9E232af676b8975150aD", 
        "amount": amount_str,  # Pass amount as a string 
        "contract_address": "0xFED52beB05e61867515fDb663276E9E458106c29", 
        "callback_url": "https://postman-echo.com/post?", 
        "signed_data": "" 
    }) 
     
    headers = { 
        'client_id': '0c4ea9c297c4351887d6cb92520f9f09198bcd03b5e141a126a73e9a021ed061', 
        'client_secret': 'sk_f2146fd8c39157338bdb66f749e03b2fb5a734b56189db966aaff82de6968101', 
        'content-type': 'application/json' 
    } 
     
    conn.request("POST", "/api/token/mint", payload, headers) 
    res = conn.getresponse() 
    data = res.read() 
    print(data.decode("utf-8")) 
 
if name == "__main__": 
    parser = argparse.ArgumentParser() 
    parser.add_argument('amount', type=float, help='Amount to be sent in the token mint request') 
     
    args = parser.parse_args() 
    main(args.amount)