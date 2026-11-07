import {useState} from "react";
import type {Finding} from "./generated-client.ts";

export default function Findingform(){
    
    const [findingDetails, setFindingDetails] = useState("");
    
    function  createFindingObject(){
        
        
        let finding : Finding = {
            details: findingDetails,
            
        }
        
        return finding
        
    }
    
    return <div>
        
        <input onChange={event => setFindingDetails(event.target.value) }></input>
        
    </div>
    
    
}