const { glob } = require('glob');
const { promises: fs } = require('fs');
const replace = require('replace');

const currentSolutionName = 'RichWebApi';
function validateSolutionName(name) {
	const notAllowedCharacters = ['\'', '"', '/', '\\', '?', ':', '&', '*', '<', '>', '|', '#', '%', ' '];
	if (typeof name !== 'string') {
		throw new Error('Solution name should be a string');
	}
	if (name === '.' || name === '..') {
		throw new Error('Solution name can\'t be a dot');
	}
	if (notAllowedCharacters.some((c) => name.includes(c))) {
		throw new Error(`Solution name can't contain space or not allowed characters: ${notAllowedCharacters}`);
	}
}

(async function main() {
	const replaceSolutionNameFor = process.argv.slice(2)[0];
	if (!replaceSolutionNameFor) {
		throw new Error('Missing target solution name');
	}
	validateSolutionName(replaceSolutionNameFor);

	if (replaceSolutionNameFor === currentSolutionName) {
		console.info('Current solution name is the same as target name');
		return;
	}
	const files = await glob('./../**/*', {
		dot: false,
		ignore: ['../node_modules/**', '../LICENSE', '../CODEOWNERS', '../README.md']
	});

	console.info('Found files to replace old solution name occurrences:', files);
	// Replace case-sensitive occurrences
	replace({
		regex: `${currentSolutionName}`,
		replacement: replaceSolutionNameFor,
		paths: files,
		silent: false
	});
	//Replace lowercase occurrences
	replace({
		regex: `${currentSolutionName.toLowerCase()}`,
		replacement: replaceSolutionNameFor.toLowerCase(),
		paths: files,
		silent: false
	});

	const filesToRename = files
		.map((x) => ({ lastSlashIndex: x.lastIndexOf('\\'), path: x }))
		.filter((x) => x.path.includes(currentSolutionName, x.lastSlashIndex));
	filesToRename.sort((x, y) => {
		function countSlashes(str) {
			return (str.match(/\\/g) || []).length;
		}

		return countSlashes(y.path) > countSlashes(x.path) ? 1 : -1;
	});

	console.info('Found files to rename:', filesToRename.map(x => x.path));
	const promises = filesToRename.map(async (x) => {
		const targetPath =
			x.path.substring(0, x.lastSlashIndex) +
			x.path.substring(x.lastSlashIndex).replace(currentSolutionName, replaceSolutionNameFor);

		await fs.rename(x.path, targetPath);
		console.log('Renamed', x.path, 'to', targetPath);
	});
	await Promise.all(promises);
})
();
