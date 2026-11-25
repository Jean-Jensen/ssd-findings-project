import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Findingform from "./FindingForm.tsx";

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <BrowserRouter>
          <Routes>
              <Route path="/" element={<App/>}/>
              <Route path="/finding-form" element={<App/>}/>
              <Route path='/user-roles' element={<App/>}/>
              <Route path="/dashboard" element={<App/>}/>
              <Route path="/audit-log" element={<App/>}/>
          </Routes>
      </BrowserRouter>
  </StrictMode>,
)
