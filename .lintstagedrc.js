module.exports = {
	'*.cs': (files) => `dotnet format whitespace RichWebApi.sln --include ${files.join(' ')}`,
	'!(*.cs)': 'prettier --write -u',
};
