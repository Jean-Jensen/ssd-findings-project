import {useEffect, useState} from "react";
import {Finding, FindingClient} from "./generated-client.ts";
import {apiUrl} from "./api.ts";



const findingClient = new FindingClient(apiUrl, {
    fetch(url: RequestInfo, init?: RequestInit): Promise<Response> {
        return window.fetch(url, {...init});
    }
});

export default function FindingTablePage(){
    
    const [allFindings, setAllFindings] = useState<Finding[]>([])
    const [sortState, setSortState] = useState({key: "name", ascending: true})
    
    
    useEffect(() => {
        findingClient.getAllFindings().then(result => {
            console.log(result);
            setAllFindings(result)
        }).catch(error => {
            console.log(error.message);
        })
    }, [])
    
    function sort(key: string){
        
        setSortState({key: key, ascending: sortState.ascending})
        
        if(sortState.ascending){
            sortAsc(key)
        } else {
            sortDsc(key)
        }

        setSortState({key: key, ascending: !sortState.ascending})
    }
    
    function sortAsc(key: string) {
        setAllFindings([...allFindings].sort((a, b) => {
            // @ts-ignore
            if(a[key] < b[key]) { return -1; }
            // @ts-ignore
            if(a[key] > b[key]) { return 1; }
            return 0;
        }));
        console.log(allFindings);
    }

    function sortDsc(key: string) {
        setAllFindings([...allFindings].sort((a, b) => {
            // @ts-ignore
            if(a[key] < b[key]) { return 1; }
            // @ts-ignore
            if(a[key] > b[key]) { return -1; }
            return 0;
        }));
        console.log(allFindings);
    }

    /*const headers: {key: string, label: string}[] = [
        {key: "title", label: "ID"},
        {key: "name", label: "Name"},
        {key: "address", label: "Address"},
        {key: "phone", label: "Phone"},
        {key: "email", label: "Email"},
    ];*/
    
    return <>
        <p> test </p>
        {allFindings?.length > 0 && (
            <div className="user-roles-table">
            <table>
                <thead>
                    <tr>
                        <th key={"title"} scope="col">Title <button onClick={() => sort("title")}>{sortState.ascending? "▲" : "▼" }</button></th>
                        <th key={"details"} scope="col">Details <button onClick={() => sort("details")}>{sortState.ascending? "▲" : "▼" }</button></th>
                        <th key={"affectedAsset"} scope="col">Affected Asset <button onClick={() => sort("affectedAsset")}>{sortState.ascending? "▲" : "▼" }</button></th>
                        <th key={"severity"} scope="col">Severity <button onClick={() => sort("severity")}>{sortState.ascending? "▲" : "▼" }</button></th>
                        <th key={"status"} scope="col">Status <button onClick={() => sort("status")}>{sortState.ascending? "▲" : "▼" }</button></th>
                    </tr>
                </thead>
                <tbody>
                    {allFindings.map(f  => {
                        
                        return (
                            <tr key={f.findingId}>
                                <td>{f.title}</td>
                                <td>{f.details}</td>
                                <td>{f.affectedAsset}</td>
                                <td>{f.severity}</td>
                                <td>{f.status}</td>
                            </tr>
                        )
                    })}
                </tbody>
            </table>
        </div>
        )}
    </>
    
    
}