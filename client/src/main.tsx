import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Findingform from "./FindingForm.tsx";
import UserRolesPage from "./UserRolesPage.tsx";

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <BrowserRouter>
          <Routes>
              <Route path="/" element={<App/>}/>
              <Route path="/finding-form" element={<Findingform/>}/>
              <Route path='/user-roles' element={<UserRolesPage/>}/>
          </Routes>
      </BrowserRouter>
  </StrictMode>,
)
