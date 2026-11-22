import {atom} from "jotai";
import {UserModel} from "./generated-client.ts";


export const userAtom = atom<UserModel>()