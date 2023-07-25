const { glob } = require('glob');
const fs = require('fs');
const replace = require('replace');

const currentSolutionName = 'RichWebApi';

(async function main() {
	const replaceSolutionNameFor = process.argv.slice(2)[0];
	if (!replaceSolutionNameFor) {
		throw new Error('Missing target solution name');
	}
	validateTargetSolutionName(replaceSolutionNameFor);

	const files = await glob('./../**/*', {
		dot: false,
		ignore: ['../node_modules/**', '../LICENSE', '../CODEOWNERS', '../README.md', '**/bin', '**/obj']
	});

	console.log('Found files to replace old solution name occurrences:', files);
	// Replace case-sensitive occurrences
	replace({
		recursive: true,
		regex: `${currentSolutionName}`,
		replacement: replaceSolutionNameFor,
		paths: files,
		silent: false
	});
	//Replace lowercase occurrences
	replace({
		recursive: true,
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

	console.log('Found files to rename:', filesToRename.map(x => x.path));
	filesToRename.forEach((x) => {
		const targetName =
			x.path.substring(0, x.lastSlashIndex) +
			x.path.substring(x.lastSlashIndex).replace(currentSolutionName, replaceSolutionNameFor);
		fs.rename(x.path, targetName, (error) => {
			if (error) {
				throw error;
			}
			console.log('Renamed', x.path, 'to', targetName);
		});
	});
})();

function validateTargetSolutionName(name) {
	const notAllowedCharacters = ['\'', '"', '/', '\\', '?', ':', '&', '*', '<', '>', '|', '#', '%', ' '];
	if (typeof name !== 'string') {
		throw new Error('Target solution name should be a string');
	}
	if (name === '.' || name === '..') {
		throw new Error('Target solution name can\'t be a dot');
	}
	if (notAllowedCharacters.some((c) => name.includes(c))) {
		throw new Error(`Target solution name can't contain space or not allowed characters: ${notAllowedCharacters}`);
	}
}
