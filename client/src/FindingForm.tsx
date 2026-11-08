import {useState} from "react";
import {Finding, FindingClient} from "./generated-client.ts";
/*import {useAtom} from "jotai";
import {userAtom} from "./atoms.ts";*/

const apiUrl = 'http://localhost:5001';

const findingClient = new FindingClient(apiUrl, {
    fetch(url: RequestInfo, init?: RequestInit): Promise<Response> {
        return window.fetch(url, {...init});
    }
});

export default function Findingform(){
    
    const [findingDetails, setFindingDetails] = useState("");
    //const [user] = useAtom(userAtom)
    
    function createFindingObject(){
        
        let finding : Finding = {
            findingid: crypto.randomUUID(),
            details: findingDetails,
            discoverydate : new Date(),
            creatorid: "9f2aa213-d707-41f6-a220-84925fb0c55a" //temporary ID
        }
        
        return finding
        
    }
    
    async function createFinding(){
        let finding: Finding = createFindingObject();
        
        await findingClient.addFinding(finding).then().catch();
        
    }
    
    return <div>
        
        <input onChange={event => setFindingDetails(event.target.value) }></input>
        
        <button onClick={async () => await createFinding()}>Submit</button>
        
    </div>
    
    
}