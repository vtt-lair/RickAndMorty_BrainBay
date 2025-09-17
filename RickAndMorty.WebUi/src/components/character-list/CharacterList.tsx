import { CardContent, Box, CardHeader, Button, Stack, Grid, Typography } from "@mui/material";
import queryClient from "../../utils/query-client";
import api from "../../services/api";
import { useLoaderData, useNavigate } from "react-router";
import { useTranslation } from "react-i18next";
import CharacterCard from "../character-card/CharacterCard";

export async function loader({ request }: { request: Request }) {
    let characters = await queryClient.ensureQueryData({
        queryKey: ['character-list'],
        queryFn: () => api.character.getCharacters(),
    });

    const url = new URL(request.url);
    const searchParams = url.searchParams;
    const fromParam = searchParams.get('from');
    
    if (fromParam) {
        characters = characters.filter(c => c.origin?.name?.toLowerCase().includes(fromParam.toLowerCase()));
    }

    return [... characters];
}

export default function CharacterList() {
    const characters = useLoaderData() as Awaited<ReturnType<typeof loader>>;
    const { t } = useTranslation();
    const navigate = useNavigate();

    const navigateTo = (path: string) => {
        navigate(path);
    };

    return (
        <Box maxWidth="lg">
            <CardContent>
                <Stack direction={"row"} justifyContent="space-between" alignItems="center" mb={2}>
                    <Typography variant="h3" component="div">
                        {t('titles.character_list')}
                    </Typography>
                    <Button variant="contained" color="primary" onClick={() => navigateTo('add-character')}>
                        {t('buttons.add_character')}
                    </Button>
                </Stack>

                <Grid container spacing={2} direction={"row"}>
                    {characters.map(c => (
                        <Grid key={c.id} size={{ xs: 6, md: 4, lg: 2 }}>
                            <CharacterCard character={c} />
                        </Grid>
                    ))}
                </Grid>
            </CardContent>
        </Box>
    )
}