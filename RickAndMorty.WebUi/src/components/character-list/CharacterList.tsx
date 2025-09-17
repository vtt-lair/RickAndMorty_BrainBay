import { Card, CardContent, Box } from "@mui/material";
import queryClient from "../../utils/query-client";
import api from "../../services/api";
import { useLoaderData } from "react-router";
import { DataGrid } from '@mui/x-data-grid';
import { useTranslation } from "react-i18next";

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

    return (
        <Card>
            <CardContent>
                <Box height={800} width="100%">
                    <DataGrid
                        rows={characters}
                        showToolbar={true}
                        disableColumnFilter={false}
                        slotProps={{
                            toolbar: {
                                showQuickFilter: true,
                                quickFilterProps: { debounceMs: 500 },
                                printOptions: { disableToolbarButton: true },
                                csvOptions: { disableToolbarButton: true },
                            },
                            columnsPanel: {
                                sx: {
                                    '& .MuiFormControlLabel-label': {
                                        color: '#000',
                                    },
                                    '& .MuiTypography-root': {
                                        color: '#000',
                                    },
                                    '& .css-rizt0-MuiTypography-root': {
                                        color: '#000',
                                    }
                                }
                            },
                        }}
                        columns={[
                            { 
                                field: 'name', 
                                headerName: `${t('headers.character_name')}`, 
                                width: 300,
                                filterable: true,
                                hideable: true,
                                type: 'string'
                            },
                            { 
                                field: 'species', 
                                headerName: `${t('headers.character_species')}`, 
                                width: 150,
                                filterable: true,
                                hideable: true,
                                type: 'string'
                            },
                            { 
                                field: 'type', 
                                headerName: `${t('headers.character_type')}`, 
                                width: 200,
                                filterable: true,
                                hideable: true,
                                type: 'string'
                            },
                            { 
                                field: 'gender', 
                                headerName: `${t('headers.character_gender')}`, 
                                width: 100,
                                filterable: true,
                                hideable: true,
                                type: 'string'
                            },
                            { 
                                field: 'origin', 
                                headerName: `${t('headers.character_origin')}`, 
                                width: 200,
                                filterable: false,
                                hideable: true,
                                renderCell: (params) => (
                                    <span>{params.value?.name || 'Unknown'}</span>
                                )
                            },
                            { 
                                field: 'location', 
                                headerName: `${t('headers.character_location')}`, 
                                width: 200,
                                filterable: false,
                                hideable: true,
                                renderCell: (params) => (
                                    <span>{params.value?.name || 'Unknown'}</span>
                                )
                            },
                            { 
                                field: 'image', 
                                headerName: `${t('headers.character_image')}`, 
                                sortable: false, 
                                filterable: false,
                                hideable: true,
                                width: 80, 
                                renderCell: (params) => (
                                    <img src={params.value} alt={`Character ${params.row.name}`} style={{ width: 50, height: 50, borderRadius: '50%' }} />
                                )
                            },
                        ]}
                        initialState={{
                            pagination: {
                                paginationModel: {
                                pageSize: 20,
                                },
                            },
                        }}
                        pageSizeOptions={[20, 50, 100]}
                        disableRowSelectionOnClick
                    />                   
                </Box>
                
            </CardContent>
        </Card>
    )
}