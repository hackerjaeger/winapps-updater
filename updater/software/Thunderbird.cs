﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.0.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "b7bed2f8a510a48060f33285f0b87c0b04fe5b86eae046f7a5bb7f5d4850c6a14cec776fddd82e5d266ee0c3bb8471d55f748862d038ecc0bddffb4fd63f42a1" },
                { "ar", "c9cbf3fa351e857c92b7ee12d2333285f2e60e02442ba20304f489ad6c6239049d0bf340c5870f9217c7a6cfd83e7bb0f370b924b16b0c73f75c6cbf5ac56bda" },
                { "ast", "6a1d77e242ca981a806d6306e449558d5fcc81480c62fb1763a83c7a4ed47d588164c8f3f8b34828a54f17a14c691c04fd32ced5226375d566ab6e64276c0bbc" },
                { "be", "5853d2b3b07746d02de47261dd343caa1f3c4015a072a738968f1a5e95d000d4b9ec0b0255e9569fba14719e279a487dae0efa5df1028fcdcba5c8f3b9044253" },
                { "bg", "0ab774a1370d556914795cf93e99484ad89ac6d1983da7a15a8dc06c49c5563856f0f82d78582d3a2b139c5590cc9ebfa4eab1df0d5a7c8eef29842481033ded" },
                { "br", "400aa003407db4048e973db63b6bdba2ba1f3a6314851cf057724753c023d4a4fd8e90a24bb44c0df537fcd837a0157ba0ee7883870c767e7237e08c02e56b48" },
                { "ca", "f5eeda7197f60502b0725498ebf7c996cd7663d5d5ef69b91ccb301c59eabc9e51af08295afe5c11ce0fdcfee326ad6d86af2c0927a7622d7f38a61093ea2fc6" },
                { "cak", "f5dac7cd79dda80fee27ec1e6c38777c8019ae9756b0543addb68e6a510319d1f28ed82e8c5e9ecf047b2889f32b4944336e85143909b1b56a4d6c0e10392cae" },
                { "cs", "b1b0a9ee7a27d51ac486a25ddd51cdbab2025479e4b3e68ab203c5908b9f5344fd1c2ae93e6fc8863b9f7835d67420a959a386b1d3b74b171d9afc8aef24dcc9" },
                { "cy", "5f8abd46550feb7c5898b956980ff39d15e6cfb6a0e51f217458d254f565359502c8938241e17b6a144797778a09476a031636b989feb8519aa6e38752e913ac" },
                { "da", "7f81adcdb265836934eec02c8b30ed2c0bd285421a0d20c216639726195d290554d96b35cfbd20bdae2d3cb19643e0d46900765be306b6d70953ae24b2c3cca7" },
                { "de", "046988fa399c610821ec90bbaf3290f403b7f125bcb89b3652e8ba14546e64983d205fd5c392ad10dc11253f9eac06df2ad5fbb0b1e6c69f3d9d96a665f82feb" },
                { "dsb", "dca7880174024ca18d33022f36dae26906e31316ac4944a6db7f80c8f8a9ca25a320d5b8fa9893155649ae9913c79a3e2a44a0ee0fb4d8f68392066f965481f4" },
                { "el", "7830be5751af4ab6c8768d99ce9a2f7c69cad00cc7cc951d642267931c61553eac1436ad4e0e1b60a41f4a627136728ec0d9ecad614b4cdd9cae32fc9de53610" },
                { "en-CA", "42513193c881a9c54fa30114d62ba0d4f11bcee6ad48e9d2c9f99595c9a3d12652d48ed2911f80b48800e1d08d46359b1de2077089681155e9419bc988052694" },
                { "en-GB", "eb4898a06900c47081cdecaac909e3b640186416cb4f70e7791f7e6f97fdc9c0571129c17f976d8a9bf018e485535f0873b58d02e7c050d32c663e8f5f20f118" },
                { "en-US", "0f35737d7a7f060ebe49c97c0ffc9abe301c26219bce5c9110d7a6af206b716ad12ce678b44dab9e8978334a60fd109861cd59536e72a34197765d803f5fca0d" },
                { "es-AR", "b82355714d14919f378d2cb3added2421ff04ffc47a5b5b03ad18f13822519a2b8b86bd42af8906aa82f5ad30586be8597257c551415330194c0d52a8e275ac6" },
                { "es-ES", "3ec6e6bf70078aa7bb82e9ef1689f73be2d1332a9c31632dc82ccb9450fea943d1414a7a4c6240a4a510e024c5a33daa7c47ff651a57b68ac4e6a5842825357e" },
                { "es-MX", "8d981127142b4c2d22992b104a5358947b4fddb6d1024dc76558558a4133463d8cd4c38c460dd376dfd6565d8b1b365cef13349530605555eefdbcc315272bcc" },
                { "et", "54ac8ee3f5b8f7bf12bf449c3f59a38942194100872331822380161aa214d8448cb6174e7d9613ff1b0f78e426fe4493a1c899387e35b59f76b54c6f622523f7" },
                { "eu", "8888dbc9442ee00dad995cb8ff0319872d39d70ddbec2ed97fab0e7b8285079e81fe19c212e1d16fdb64267b2faa41568fe06f202788daa18eba66fc80265275" },
                { "fi", "268fea80564eed1e82e027be95a7b780cc7d445f9d2fb3ae86c8fb18f3f4aa0b0abad60f3adce3f166143d8f7e1607cfab5fe7ce5abdfc8d7702fe5ddfbcc9fc" },
                { "fr", "7cd4837906ba8773e5b2bc07059afe325d905cb2f4a2dcb0fb8d72596bda026fc4979ff51e61ce7f7b273bf4356830a5fe747e56c0ae781cf3662c36c1a8d711" },
                { "fy-NL", "95a12b6bace00f78802328cf94b056ff96044566bb539a0c2bd5c7ec8827a6e55521905956f784d894dd0862c8dbd533e0f3fad4f6d930baaa30746c6b2a0eaf" },
                { "ga-IE", "fdbf9997ed51ecc34d7e45efebac7c14cbf5c8d7ba4ce6c2a8e002af384e7f2ff681bdbe77e5f4ed6236a95005df530367a7707c67458cc7844f05a888d216a0" },
                { "gd", "9493b5e7aa2df8991610125939c20e96484977aa4db132527e372286edac78c1878894cce841dab56254c4b9e10a45ca558282f9646e6fb5951a051f15e43642" },
                { "gl", "54d646cf0f9bb3c1172a9568889ccdc7da635d18ae1721125d4a92594c1c8f91ef8142cf627789f8ebe8dbc147c1b742e494b8fb7102df2196155afef85ccc4b" },
                { "he", "d80632903da5a2f9d5b0ef706e8055f9db123708293b45b0c439c9553e739db77ff0333a810d734069caf0f1fc46221f7117274953b64c2a2c72facd3504b7c5" },
                { "hr", "9e8a77761ae158544ce0e53c2ff07cb40d91e33caa84d1aa588efacbf794d882d8a471f68c3cd7a55146c56cd2e2b500e737155e8e685abb023c74e93cc649ce" },
                { "hsb", "a9eb8eec7ad6693e5fe5a203e3a3792e479e0b7fcb944efc4aa42d381ec80968c8e61ab7951f874af3828a111a7f341d25d63e8c1eaa11809d842ecdecfc2c1d" },
                { "hu", "1207be9dad0b9358fa05f86130b13c9b36e8e9af52600b622b7573bd0ca06ac5d27b15d47acd9b039802c65bdacb6c9cc56c1e89c7369fd760a478a9e218f83f" },
                { "hy-AM", "d0ca61db0350c4842a6f0530bbe74d3d19244a86d983090c2891b27a9412e9d4e51f33468465e0d444fca096ed547d5eca1d30544890de7d902687a3e4d46fa2" },
                { "id", "d3f57c56e26e6782c65752c37331b52c69a8c57f0f112bdc18d1d322722727441b3d48f895f3d73b6f9c958d5d689c4505541e66100f47495abb3a88df651409" },
                { "is", "01bc45da47cba26a4118a6ccbd1d7e1a7fe0209d8a19b3166753b1e667d717bfc983c6f4e3b8ff57072ddf8dd80766bf3c58d22eda179d68cb5da88ff2df482a" },
                { "it", "b83e864bdeabac6495580f1077c4e7bbb95e207516621727e3addee25694bb38eaf4e1ec2d6d8e2d99efe51baee7f92f376077d7cadb2c2d0f840151394a71b2" },
                { "ja", "0a3d1c5b3ea8df148675879b50b0f6b80d228c1a8c4225af49252d37816912c2f806f6a4726000ccfbdcc332cde87838bb73586881918aacaa5614d3955252a0" },
                { "ka", "f1aa824d7fc24f137975f1bdc0ba9782f3d705fd56caf37fff20dd6989c43bb8b28b46111b1e936baccd3bf35a9153e74a8f9e4b45ec5b83a00b7e6faa6166c8" },
                { "kab", "4aea0b1d7995be66178c284ab02ff7a33319acc26c738409a36dd81ff849d9007da561b2135f408df8b455da3fefa470b81ba8ce18b032ff97df1991f73438e3" },
                { "kk", "d78c626e1cf86a6314bf668ff3c42fceb0b445fde453d2777c92d09ac9a18990923c26d3c15ebdc5129edc1531d0d7f1bb7d0b127be0035997f36c57a7a7a05b" },
                { "ko", "3af1794311ce6456933f47f37d681c30552e3ae092582727df9a0b9ddd4760a3519af4e7b12bba37b4b18547cecb1fda1b40e8ec91f48232aaa72de5708cc58d" },
                { "lt", "a4b1728396f88b6847930d287b4ee921f31752b624cad0c0a3059e87047abd869ccc3e5213b115908572bbe581a773afaee83c3ec17d1a483bf530fe90ea56ea" },
                { "lv", "d0a9018f317cebc9f5982fc8f09c2b1ccb8205afeee86609994a009730a5754b23015aba5b81cdf7ac82dca1cea1e899d37ab5923a73ee79678959e03c26c3ca" },
                { "ms", "37a5cffb1c390572ab3cd2b7e5b419b8d5f3a7991ca1380b3f46bc3fa46950276d336284393167dd9082b618628420b4b8331699ffa64d0a3f4587f58258eba0" },
                { "nb-NO", "5a08d4ddc37c25cb77fb9effe9534771c223a0705896872abbcc548fe9550fc2616e1b9e843f1ec896e2eefe8484dda7af2d728f8da8334138ff35c33b288455" },
                { "nl", "e6405f2f5bab5cfc5253074c5456ad4fe799ca298e2283e79a841cde875140fe3f1f2776534dded4e18f0eb0e32f329b702845910971d1b1a5123a565e3d59c0" },
                { "nn-NO", "acc8062aba429a8fd50469aefa32e41de5afa1f6432d1522c62807b5d4d6bdfec6e72604e82d7b3b7983aa59706c3af7ad2d403b275d2baf4e2c14d6c4f92d73" },
                { "pa-IN", "1e913d6855bfba920b6433d63f39b8b64aa0f39a2507fadb5297b8b5413bb580f62ba4e32889330077a307b90e5b015e40d8c77e38367232d6531fd5c589716e" },
                { "pl", "e073257871d35a3b894f3340defb72bcce3a7e4a74acdcd48ba284b689d76799164b06bcad3957ca9cb11f12ed48fc359ff5d4203ded5a315e08a835510d05c1" },
                { "pt-BR", "dd246c2154f1974604ed75b02538c00e8760eb78eaa50b6f4ded3d18aae26935a19df152a7bad4532350b204d91a876ec6b2e051ed039cea7c5c7287531019b3" },
                { "pt-PT", "70a0658cf5cb87559ba3cc6bafd7ede86dff05ec9ea2ef21e812044f29c54f460fb9fa5a3d2403ef4663fe760a42765611a348c1bb8da94186328f4fb7271de9" },
                { "rm", "367e34ba3dfa72512983791801b4c1d5596c7dbf45526d0b83e620768f53131895232a1bfc37cc3d5ce93dd2fae900943aa28b4f9b0b5adcfb32dda663d7b57c" },
                { "ro", "ffce664dd2f519b4bf99c83296cb4fdbc3d47a153f8178356cb94ffb6eb27841c9e77b7184473bd06bcc8279b36d52c6367afedb52fa929971fd7cf7fb4d816d" },
                { "ru", "3b5a7cafa8d0aa8892c4be903db4de5a7cd90a27de5f97598608c290bbf611499f964e9a1ffbf506179ca686beb4e2000465b3160bee0da02ed2a7c43c800980" },
                { "sk", "588d829044ab857dc4d7efa0cbafa0694fab184efd3647bac40fb9bf30c4f927454d81563e4259bdcdec691ca1d50f14f326dcad8224b53925afc5e6e5a887a8" },
                { "sl", "14762bc1869e8947c1f63259a62439ba4197e90083523262d2dceb9ee6d961319382830a708b3496017de589d38976893982c193085f9268c1b1cbaf9a908b8f" },
                { "sq", "b873812d50e9b9c2fd232fc23ec51f4572f69f36e8d4b9d05010e716e7419c4ff30415cbde110ba6a8b907cfffdabd5a58ff130322d250ac0b8803ec465cbc57" },
                { "sr", "1476acf8bb411a271ad2d727985bab42db9f6f965b3246c83f5417a144b8e73a06dcdcbed70acebb1724c98a923cebbbb9e288c54ca182a4753847762a8f56ae" },
                { "sv-SE", "dcbcea16e20034b0270e6bcf00bcd545b5ca841d5befc4769722892c2b750cf4bea2116dd58c547c72eada1921d8e8f0e05444f3fdf0c6e074bcd2e49650fe49" },
                { "th", "239515fcf1343334444c1402c63d0bc8c2c358e87cd297dabd495bfdf0cc77b89d6b8bf40c5454b35ae1922712dc90490547b4bf7e506c45d40ea7ab32676259" },
                { "tr", "1a8c9768565e47af3b95b87e59f717884a248b679187b81c68526e00c6de65d1e7c9876be2c1a77b67fef8a35382082456e38ea2fac2eee7db5adb26ba5a37f8" },
                { "uk", "7d3a57ee491175494bf0786816be6fd911addaed9ad09b2f7cd086a4f6f9c5cd39ec0552f0841a4970a63b702d562fda295875519467238497d3b5401df6fea6" },
                { "uz", "a425f3ba936783aa0227257ca1782b60b2f3ec1f9c01d13aa04c9dcdf2640d24dcecad0b988c2055c6395e102113f12545f0b71c6bca2989359253cca4a5f844" },
                { "vi", "9e54898320f0992c7a12dcad8a37ca65596ab51373690fcbed4e7131607cac498ca4bb46760e1086b64781ac770b9f86adc9c36e599a1325322aa7df160a1101" },
                { "zh-CN", "37d3919d03d7577d2a1313b2355fee9bda7020762ef4a404bfb975f6d9ba4d6834912d0d31fb0aec2a0e0b7daccca56b40e64044b4f47bd139a32f6b57180eb2" },
                { "zh-TW", "77d4cb473caa2425c22e237cf81555fde518983443c415f7d94c8d8bad525b8ab0690f5b257783b66715dd5f050e4f32f354c62ae80c340bf444fd7e92f724e1" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.0.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "5a8d317732e44d906849fcf0e7e240d6f37b06a1866ff9e22c3870d434cfedad39d712d9625672b7acdeea57d749d015ec3ca8bde1c67878293a607dcf738ab4" },
                { "ar", "59013c577f1de04e4fed7e570077405a365ca478022f7fdfff721085ce229b23bd429970fa9a91d3c31c097255fd8d136dce676bd21e12b0bfd5a78a7e837980" },
                { "ast", "61e29d493ebf876931302fc3ec0b3240b8f571f2a1e166289704db28791b387e958b60a7c709f6fd9b56141c563830350e7a4f6b8ed4bf481be8a4fbf28704bb" },
                { "be", "4039f57dd0ec02df0e422282e849bbe837da10c83ef7f93909b8d5a68d5be8cae3ae90ce2771a5b2af75fafd467c63da44055d76d8e1e1a6124b0f6c22c2c23b" },
                { "bg", "f2c9996ab28fe66fcb919186746749182066b6fc5e758f4ea5dc0f1c61f82b14104db997eadb0dfd5a11987b7c1c57054e3411794d69767c768b438703ceeee4" },
                { "br", "2ce9393f19e786ade3e2dd887c33d1d8bdf9ecd67a60504d4c5c0b85347096dd650efe9e369d45e7070de79b58902623493911a3357c0582a06d23c9eeab36b7" },
                { "ca", "6725f82799d9b84ddcc1ad9dedf1393e7c7b32b7ebc64f8d3943189804de89107a27efa75eba767bcd016359223dab6e92c16a39a0f58cf84830235d22e1001a" },
                { "cak", "909d369c40ac78e2cd2381db04ad028289613cf20a93d0c9b7cac0d1fa76a8955cacdf6e45b0d0c7e13bc341ec871f0ef04efea34c97e3a0e56eb95546093e92" },
                { "cs", "7e57d90813829f3a5bde9515294e4df4de067d852586c17892372af179309f4dfdc283b5cd9593ebc8cfb31c9156651480361f5846d52b76c356235219e7fd5d" },
                { "cy", "3f45ba2f8ac1089080dc4997a7fe755d98e88c31dc659cbad88b1d7ae9a0d325e793a7d196760f65d291f5b3c62bd6b532e7f77cddd067d8ce8f331b8cbb0988" },
                { "da", "28980277952b6df2bd26b99db6f48d787ae7322b456e32c8baf4916719891a66ff1a85171559207307914fe04d80c261512fb1e6d07daa7e8a56d4891e855e91" },
                { "de", "dbc4526149acb8cec7dd1571fc2196a574cf03b4c66958ad431ff44b6524fc8d499fc1f511a1a70622f5912f0eb35f3d75415a322f8653c14e10cb8070ec337b" },
                { "dsb", "a524151d584892a507724019124fe49caa7fa07ab2c8c46097f4eea41c8da5ebf54b59f30bec75963ed75d5c8efd9f7402d33d8b3e8c3e2e73cc8126d04e3cc9" },
                { "el", "4b5f36e48843fad835d7574bfa26a450cd6384d38e5b94b47e6d6a6f095c3cc3ba87670028c557b424b6e82330f40b2de36cbe802700284b2b4474b1f060df58" },
                { "en-CA", "2ece51b89c700e6c14199c531fd0c283153709f38134f488f8364dbcff0b618fee45de57c3ca0f8084f381b73d36a75d8cfd42cf2d59ae71acf55cec9c3a8734" },
                { "en-GB", "58f40c58871feab21dfd7e7b2fc88986e4cc21c84e34942b44dd16f13c1206f394513c9662ae39cdfee3845bed10d71c65326878bd2e203b88dba65c35c38eab" },
                { "en-US", "035f40c497fe95da1c383bb1fd9c28b37f0cea300957550927f847017cf8b99e61b6d704ec11507d4a7b1f9b0d6918499ab1b8e493db47d984bd6781f165a333" },
                { "es-AR", "fcf08c99c1ca35492d2dc4897847b3258d5c8607f4d8b90b731280a7273034fc48289caf9f6d17cc399632096f9569a1c79f9c254af4ab9ab7b0146177993e85" },
                { "es-ES", "0217513f2ad2b1bd0d679ab480069834cc982d93150acbc0ee63b8ca208bc9319835e930dbd6ba956d36886ab7f890e676141269cb4d99f07e7c82798a95dd18" },
                { "es-MX", "a71bc2ca3219441da707281e0d46f5ce6d5b408e3102021c1ac2254ad44d9091b196aa0933cc00b263207d9e0b900d397a35c6e0f23a38a20a2714e538076d6a" },
                { "et", "a539188814967e0e9e22506bc54849efba4516ac340f64c7cdec1362ad0efd3fb8bbf6f759e9a417f66fc96b4fcd42776d049a3fad482b5d869a085745e6e400" },
                { "eu", "730d54d07f4b7f023ea8e2ec9ccc002431f8fb5df716c54f78742d29a3cad971508b409ad394fedc89bb9fbbde09c4b9e11b54920b7a88f08f5bc5121cd70874" },
                { "fi", "a19340a859a24a11e3d019ab4e9bbb3251249d51c1962da6b5747c6ef0291b9ec3e859cc0ed8acb1000287a52265e569fc1552391e8981cde56744de911e4f3e" },
                { "fr", "9ff20796aeef54d35c5206a607ad6b86aec9cb6981f88bd068815d74bb422e4cfb131019f6d8ee7f1f755c91a6827e140b7da921b32ae7cfc16ad55a2d969376" },
                { "fy-NL", "df14dcbc731fca01f930ac52a3a9a575e3b9cf99ba0a146a9d6352f551afff02031dd664e55ff6b6f32ec2b8e6ca2643cb1fc15d10ab3ad3dad20d5c4a3e7bd2" },
                { "ga-IE", "3daedd34f7ba6fb3e8bd317aca476b6c530dbf75a2d1ce530bbe6b90a35e804dd52110ccb901004a1ab706e9e9abf10e8a92c93e2b3b64853729d5020d517a18" },
                { "gd", "ac518706ebdf6c00f0eac97bf67ffb5226b0d96e3596c5c48fa9ea63675011daee31836dfcbcf6ce94da45787b65d6aa29f5ec2960aab55619a4c88795c244a8" },
                { "gl", "e20da69fef450d3a21e92b0e32f0835e21d064339a6afc0ba5c29b3acee0aae316a33b2b2c90584ecdf64d3b6bdd084ead665cbad9eabee40f75b5e2149f67de" },
                { "he", "a657a55e896291051ee05c1f2aa65ef2829d6b3e68daa1094fefdda647f62c2fdf49f91b11bff231d69ab67b94f71ac2df1cdf9ccdb3dd26b26353502554a345" },
                { "hr", "5cbbcf575c67612c144866dbefbedf7b65e37a10d2e86d1d13479558f8af344e0014575509f2ccf44bf00077fcf545aa2bee948a4b5cef9957fe633bb197420c" },
                { "hsb", "a151f80bfc67b067c4e93bca61e71afee2a7657a41f30b7f71565b0cb5aa97356b20cf90258dbe5d01750940017689c1004c37a63d79f34d453346391495827d" },
                { "hu", "ca6c8cfb96ab5b33b3cacf4bec0d8a65c74b5a3cbe9f66f16ef37bad860eeba7a8bb1d48a953d926dc6f3beb4fd7b77efdb7bc9b3c023fccaf1d460d5b247f15" },
                { "hy-AM", "510276293846359a837b920c83cbc64968925f7ba0c95de466e6149d212918d73ec8ed11464b305c384c6238f430497f41908dc0e7d4ad3570b1364d20b67b12" },
                { "id", "26001714434e729c089b78f9bceae9e1c58fe3ed0393d6f45ec4b4bab420712c37869f8d89bd83471118a9643de9bafbd8e467712ab6ad10ee2b99abf1bb7749" },
                { "is", "901dd55bf73f922b2c400b8405f950dd191c28483816fc515a65ef1ac9d7c5bddaaa473e9e695203e5b5c5e5eef18eba2e655bad5c9a73ba90d7a28d7e2eebae" },
                { "it", "e687263908ec7fdefe26020a93e244d24408cefdcd07965fe42386a6fa048bb3f4b152f87e01738f580f11e115fcd47f6474d7f78ea088c466431253e08f5e44" },
                { "ja", "02713d09af31ca3180a17cc675ea4a980cf94cbbad48eb4b17b604ab121a3f75fe23fae3ed4869e251ed38267e3e7c03955646134a217fe6065d3b43399f7506" },
                { "ka", "175d3d56c3f31322d3b811c344dd35741edcae8841cf2761572af7d0ec55e3166ba8c7b3d290995a2accbb8e3259d45f4d11958ce37008e0ce6d821471c0a480" },
                { "kab", "68f0c48d73c6584357704bdbacc2f37d84ebc000be35912f7e4f232cedfd4edd8e39ab9642e4bf2ccc0b81dcdaad8833561b8a15f1d0e5e7ec6d06fbe53a2747" },
                { "kk", "e311e3548d9d3c4292703a18b7cc61e747cc257bee2c44e2f73b57f63cf13d2bee0ecb81f88333fea3cd3ba7cb52246b020e7f87aab649a23a15df501e709c18" },
                { "ko", "a5f0dee18a45791ace5332453a3d84890d0f75fec079d632136fb65aa13b8db911c0cb6e9be86d4896b859a69372674ea36bd2ecaba0e50523fc99b048464189" },
                { "lt", "2048e15454d54038b1286fcc497fc1a590655b86cdbac85e6797619a610fb7271a61fbe9d074a6c74cf9d725d9555eabc16c1e0653211926e2c51c5405a3a2a5" },
                { "lv", "489e9b81a91f482eabd196f6eb716e1c426be8f8e70894c1544163c6e2b2d9b23df5393e6c91144de252cf1424d13f172df2c3e1dbff9df3c1ed87c2bf135c60" },
                { "ms", "5810504de07a5302fdb3fb7804818de536e30652e8c2b743b518a4920499ac32e5165535aebb0a7d8f3120ba36fe2aa0b2c1e5c1f83d6f6f9c01591ce29188b6" },
                { "nb-NO", "5aaef71fb9ef87fbf10d7cf2df2ea716928e495c65fe57aaca467a3cf3b19852bbf1619c0ec532d7fcfbb01087c7b220289334da888d0108866fc57cfc2fdc39" },
                { "nl", "45b20f441b42789ea70d2d1f23caea63e4868fb9643964b340e381501e8543eca9e2c9376d522e8f50321df30856add4876ec4beb5af0d1427466b0cc312c6b0" },
                { "nn-NO", "44140ecfe0d14566547d58aa42b30fab94923eb8bec537ca93224c079d44320ba129e100e74a8fb52ab75ccae24c818e3c37247e8c036340ddebd57862f536d0" },
                { "pa-IN", "b66e03a9283ca6a27944aeb759300f290d60baa4919436356447f981fa801bbe9689efd3a5c02db3d78eba29dc4998d3222557b98128f28cae1ab1dc51f3e91a" },
                { "pl", "8325bc21a43af6c29e2ef8b7b7833ac9a14113de41c2d9c5979acbf7d08beae06596d252056c785376a4759b745d4e2e157359f1facc7ae7a1716ed54e175976" },
                { "pt-BR", "4ed64924a75fdbab64716a73d6e3a1c466aaaa39a38172045ff2e5301ab2c762498574829bc7044a819a9c9add972d1c3d2e16127196437e9d49764e00d5d1dd" },
                { "pt-PT", "b5dbf9f598a060b13c7d70567444cf65bffa01bc8fc1e2b7a83ce81a5c169f4313bdef93158c69739eb3b481716e6eceba104ef730ccc1ac174fee8774531e46" },
                { "rm", "f0e395471c21cbf3874e341e7aec4beb218b454cb4e6fd263c07d17278c3d7cc925c075a60d13567049c2cb43d094c76bc9aa48259820e82c4a17f6dfe828f92" },
                { "ro", "f4db4634375d6524966167e76ddaba7fab3c004824538bc0734a6ca31af884bddd2f531f970c8cac4989ed14c63f19c08eddeccc3a4d5481f0c8666b5e3797ce" },
                { "ru", "52f64b691e1ebba200d12feb1587e62e5c5fe913f29e409e7cd45b6188ced1119a179612499051d011f926a696a6efe9e35e4eade1d8a60116ebb83f7ee902d1" },
                { "sk", "bbef71e494fdc728d6a0fdf9da2a96f1fb57f465d162b2ff5b3f453dfe292a609fc580f6c5ab6c0f8427d6aa908623a04bc8f7bff04601f338743d00837febd5" },
                { "sl", "de0959499770e10bd12eca5b0a1784f9b262d19cb8797645d0192e6211bebed15427fc22e5cb1b537452b0f49911e080b0023b1ff6359adfe01fcda162eec981" },
                { "sq", "e0490213dd994b2175432f16b2fd8583b5981971e2e113a78e214e68813dd53fd1b2aa21d1c68cfafedf20b438a101c919af0ac1b50268e8c340dfd666ea3b8f" },
                { "sr", "d575a8fdd68074d18208587d4b1d590cb3be759bc266c9dbbcb1cdecb12017d06e01792dd6d3abaed9e76b5d3f9b1d8a31a35cda55edf246d92dcf6110843030" },
                { "sv-SE", "1ac51700cc7bfe2a30968b40c2452a13602d97353f713cc8e3efd55c6880dd8c0626492c9d8b5ce946993c036aca1db00184cb7d0bab3aca3448711a277e7b69" },
                { "th", "db8666bb1f13cfca673881e68bbdc7f6578cc49bca026640e199816f7099c19833490a55328e93deab729610fb331e48fa247007c5f5af27e41799fe92886ea8" },
                { "tr", "32c91da5a38f0bd26f139dcf560a15fab2b702b740e5c0da42d8a8336e6c5147451074517b6f883799b9be13a00d063b4ad3149d070222b09366c9316b430e17" },
                { "uk", "a304c2c5494720f6e23564b14649b3dcd33bbb7b1f4a5c3fe6e99057992db95579682a75daefc8a0ebd39d1359495fe57c23f5a2379ac3ec746aaf718cd7487a" },
                { "uz", "509614ef621f16aeaf03d7301bd7c029057f50fc42a54e850dff58a28724450213796e4e76b2127e900bf7f9ab12357dd33f7af74a5c02a5ee83c26c6a5a0f77" },
                { "vi", "a9ca0836adf23508d849d38ba30839399277842e8f1b7482bcffeb09a93206f68f3fff3369be8620edaca96c0737cf23b519768b01a210850bc5a066e0fc7293" },
                { "zh-CN", "be4a499fb0697754de75dbe17684d7e749ded0efbc47f5673ba05673a78e236a120ccc74b3868a2f99f99898178abaf3828ecb86ad016dedd6bd59193876116e" },
                { "zh-TW", "a5f8d79ef88df14af392eaab1bab8179a2d5a3eca1c57b585964e922fdb3c937a609d2eb6cfa28fbf6ccfe159ec370c958eeb2216a401e6811e2b36b65c44ab6" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.0.2";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
