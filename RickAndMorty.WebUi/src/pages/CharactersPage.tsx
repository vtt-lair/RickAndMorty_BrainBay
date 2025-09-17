import CharacterList from "../components/character-list/CharacterList";
import Page from "./Page";

export default function CharactersPage() {
    return (
        <Page title="pages.characters">
            <CharacterList />
        </Page>
    );
}