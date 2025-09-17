import { trackPromise } from "react-promise-tracker";
import { Character } from "../models/character";
import { get } from "../utils/axios-utils";
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