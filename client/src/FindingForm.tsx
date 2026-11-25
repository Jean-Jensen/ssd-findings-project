import {useEffect, useState} from "react";
import {FindingClient, FindingDto} from "./generated-client.ts";
import {useAtom} from "jotai";
import {userAtom} from "./atoms.ts";
import './App.css'
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
    const [titleField, setTitleField] = useState("");
    const [assetField, setAssetField] = useState("");
    const [severityField, setSeverityField] = useState("");
    const severityList = ['Critical', 'High', 'Medium', 'Low'];
    const [externalEmail, setExternalEmail] = useState("");
    const [user] = useAtom(userAtom)
    
    useEffect(() => {
        if(user != null ){
            console.log(user)
        } else {
            console.log("couldnt find user, assuming this is external reporter")
        }
        
    },[])

    useEffect(() => {
        console.log("atoms been set " + user);

    },[user])
    
    function createFindingObject(){
        
        let userId = ""
        if(user != null && user.isAuthenticated){
            userId = user!.additionalClaims!["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
            console.log("user ID: " + userId);
        }
        
        console.log("Severity: " + severityField);
        
        let finding : FindingDto = {
            findingId: crypto.randomUUID(),
            title: titleField,
            details: findingDetails,
            affectedAsset: assetField,
            severity: severityField,
            status: 'New',
            creatorId: userId == "" ? undefined : userId,
            reporterEmail: externalEmail,
        }
        
        return finding
        
    }
    
    async function createFinding(){
        let finding: FindingDto = createFindingObject();
        
        console.log(finding);
        
        await findingClient.addFinding(finding);
        
    }
    
    
    
    const handleSeverity = (selectedVal: { target: { value: any; }; } ) => setSeverityField(selectedVal.target.value)
    
    
    return <div className={"finding-form-div"}>

        {user != null && user.isAuthenticated ? 
            (
                <h1>Report your findings here, {user.name}</h1>
            ) : 
            (
                <div>
                    <h3>Email: </h3>
                    <input aria-label={'Email: '} onChange={event => setExternalEmail(event.target.value) }></input>
                </div>
            )
        }
        
        <h3>Title: </h3>
        <input aria-label={'Title: '} onChange={event => setTitleField(event.target.value) }></input>
        <h3>Details: </h3>
        <input aria-label={'Details: '} onChange={event => setFindingDetails(event.target.value) }></input>
        <h3>Affected Asset: </h3>
        <input aria-label={'Affected Asset: '} onChange={event => setAssetField(event.target.value) }></input>
        <h3>Severity: </h3>
        <form >
            <select onChange={handleSeverity} id={"severity-select"} name="severity" aria-label={`Severity`} defaultValue="">
                <option value="" disabled>Select Severity</option>
                {severityList.map(sev => <option key={sev}>{sev}</option>)}
            </select>
        </form>
        
        <button onClick={async () => await createFinding()}>Submit</button>
        
    </div>
    
    
}