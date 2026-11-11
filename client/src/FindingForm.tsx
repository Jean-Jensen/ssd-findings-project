import {useState} from "react";
import {FindingClient, FindingDto} from "./generated-client.ts";
import {useAtom} from "jotai";
import {userAtom} from "./atoms.ts";
/*import {useAtom} from "jotai";
import {userAtom} from "./atoms.ts";*/

const apiUrl = 'http://localhost:5001';

const findingClient = new FindingClient(apiUrl, {
    fetch(url: RequestInfo, init?: RequestInit): Promise<Response> {
        return window.fetch(url, {...init});
    }
});
/*
const findingClient = new FindingClient(apiUrl, {
    fetch: (url, options) =>
        window.fetch(url, { ...options, credentials: 'include' }),
})*/

export default function Findingform(){
    
    const [findingDetails, setFindingDetails] = useState("");
    const [user] = useAtom(userAtom)
    
    function createFindingObject(){
        
        let userId = user!.additionalClaims!["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
        console.log("user ID: " + userId);
        
        let finding : FindingDto = {
            findingid: crypto.randomUUID(),
            details: findingDetails,
            creatorid: userId
        }
        
        return finding
        
    }
    
    async function createFinding(){
        let finding: FindingDto = createFindingObject();
        
        console.log(finding);
        
        await findingClient.addFinding(finding);
        
    }
    
    return <div>
        
        <input onChange={event => setFindingDetails(event.target.value) }></input>
        
        <button onClick={async () => await createFinding()}>Submit</button>
        
    </div>
    
    
}