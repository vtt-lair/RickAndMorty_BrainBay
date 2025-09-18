import { trackPromise } from "react-promise-tracker";
import { Character } from "../models/character";
import { get, post } from "../utils/axios-utils";
import { getBaseUrl } from "../utils/environment.utils";
import SnackbarUtils from "../components/snackbar-utils/SnackbarUtils";

export async function getCharacters(): Promise<Array<Character>> {
    try {
        const res = await trackPromise<Array<Character>>(
            get<Array<Character>>(`${getBaseUrl()}Character`),
        );

        return (res) ? res : new Array<Character>();
    } catch (e: Error | any) {
        SnackbarUtils.error(e.message);
        return new Array<Character>();
    }
}

export async function saveCharacter(character: Character): Promise<Character | null> {
    try {
        const res = await trackPromise<Character>(
            post<Character>(`${getBaseUrl()}Character`, character),
        );

        return (res) ? res : null;
    } catch (e: Error | any) {
        SnackbarUtils.error(e.message);
        return null;
    }
}