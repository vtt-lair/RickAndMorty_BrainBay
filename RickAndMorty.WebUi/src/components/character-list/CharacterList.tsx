import { CardContent, Box, Button, Stack, Grid, Typography, Pagination } from "@mui/material";
import queryClient from "../../utils/query-client";
import api from "../../services/api";
import { useLoaderData, useNavigate } from "react-router";
import { useTranslation } from "react-i18next";
import CharacterCard from "../character-card/CharacterCard";
import { useState } from "react";
import SearchTextField from "../search-text-field/SearchTextField";

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

    const [currentPage, setCurrentPage] = useState(1);
    const [searchQuery, setSearchQuery] = useState('');
    const itemsPerPage = 12;
    
    // Filter characters based on search query
    const filteredCharacters = searchQuery 
        ? characters.filter(c => 
            c.name?.toLowerCase().includes(searchQuery.toLowerCase()) ||
            c.species?.toLowerCase().includes(searchQuery.toLowerCase()) ||
            c.origin?.name?.toLowerCase().includes(searchQuery.toLowerCase()) ||
            c.gender?.toLowerCase().includes(searchQuery.toLowerCase())
        )
        : characters;
    
    const totalPages = Math.ceil(filteredCharacters.length / itemsPerPage);
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const currentCharacters = filteredCharacters.slice(startIndex, endIndex);
    
    const handlePageChange = (_: React.ChangeEvent<unknown>, page: number) => {
        setCurrentPage(page);
    };

    const handleSearch = (query: string) => {
        setSearchQuery(query);
        setCurrentPage(1);
    };

    const handleClear = () => {
        setSearchQuery('');
        setCurrentPage(1);
    };

    const navigateTo = (path: string) => {
        navigate(path);
    };

    return (
        <Box 
            maxWidth="lg"
            className="character-card-box"
        >
            <CardContent>
                <Stack direction={"row"} justifyContent="space-between" alignItems="center" mb={2}>
                    <Typography variant="h3" component="div" className="page-title">
                        {t('titles.character_list')}
                    </Typography>
                    <SearchTextField onSearch={handleSearch} onClear={handleClear} />
                    <Button variant="contained" color="primary" onClick={() => navigateTo('add-character')}>
                        {t('buttons.add_character')}
                    </Button>
                </Stack>

                <Grid container spacing={2} direction={"row"}>
                    {currentCharacters.map(c => (
                        <Grid key={c.id} size={{ xs: 6, md: 4, lg: 2 }}>
                            <CharacterCard character={c} />
                        </Grid>
                    ))}
                </Grid>

                <Box display="flex" justifyContent="center" mt={3}>
                    <Pagination 
                        count={totalPages}
                        page={currentPage}
                        onChange={handlePageChange}
                        color="primary"
                        size="large"
                    />
                </Box>
            </CardContent>
        </Box>
    )
}