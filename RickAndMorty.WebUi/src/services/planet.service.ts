import { trackPromise } from "react-promise-tracker";
import { get } from "../utils/axios-utils";
import { getBaseUrl } from "../utils/environment.utils";
import SnackbarUtils from "../components/snackbar-utils/SnackbarUtils";
import type { Planet } from "../models/planet";

export async function getPlanets(): Promise<Array<Planet>> {
    try {
        const res = await trackPromise<Array<Planet>>(
            get<Array<Planet>>(`${getBaseUrl()}Planet`),
        );

        return (res) ? res : new Array<Planet>();
    } catch (e: Error | any) {
        SnackbarUtils.error(e.message);
        return new Array<Planet>();
    }
}