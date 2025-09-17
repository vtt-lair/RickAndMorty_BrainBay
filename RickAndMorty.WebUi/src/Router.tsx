import { createBrowserRouter, createRoutesFromElements, Navigate, Route } from "react-router";
import MainLayout from "./layouts/MainLayout";
import CharactersPage from "./pages/CharactersPage";

import { loader as CharacterListLoader } from "./components/character-list/CharacterList";

const defaultPage = 'character-list';

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route element={<MainLayout />}>
            <Route path="/" element={<Navigate to={defaultPage} replace />} />
            <Route path="character-list" loader={CharacterListLoader} element={<CharactersPage />} />
        </Route>
    )
);

export default router;