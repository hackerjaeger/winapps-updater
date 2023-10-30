﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "120.0b4";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "977aafafed14c624063c6bcb2f9bcb0913c2bbf2f9d57c21ec31d56c7e97743b237794018e33e767079b4c5f7f52f7f634ed4df95388a0dbcafb96598138d9dd" },
                { "af", "c70b7580208cb0d8e3815e0b361354c66e7814770975dc0fa8ab072ec063e6b53ede891e1e922404326910df0a37dcca08cd71d12f3dc1c5d5d41b9e0bc78a24" },
                { "an", "35f7ce29167e7de68b290248116d88e52bf350382115e3ab9ac8b760a3b45e8a6d34a0b05834d4ce902a98d8a73b493e8f42aa5041621852ea16ade4d65d196f" },
                { "ar", "8c9764f5ce420dc3204fa0899a39133556bec21de136acebd79ba08b35bc875a264cfe1c3ebe3ae75cdf14289daeaf208ab88579e89779c74b05da913e0b35f5" },
                { "ast", "b1a2d8171b20f21c064dbbe31db186f066edaad07006a429dc55a395fbf8cee49e019b017032812239e5c21a44389df9c21e41ec05a97822b44241c057fe50e7" },
                { "az", "a47971113e8c10c9b5f6650209a503d2cefd91e0020288ec7347f2af696321ede652aa381f409248e73ed06c7f79a2f0f007f8650950c2c9ca005bdfce20c0c9" },
                { "be", "cf3fd91cb12efe71fcc41c67845797931bdfa1f688a593fc664350bfbb9e2f9cf739cd516451cdbf06a7baa07dd5a5d2bfe9567486530364aecfd90872ab8a10" },
                { "bg", "f99c51a86db6d9a8ddbb3227c0c031f3d5ce4ad901caf2d11881d27160bf0eb6beb394f369e0f63fbace304ae350b2860278e7e713e16c42b69aef5c35a67202" },
                { "bn", "260c028a19fd75b0ea8bd512048a8ea4e86ee22a8c5c842c9e96312fa38fe6822f9bf1982f399e08c421234e22813291f5af69c4f298b5fc7115903a8f8ecc9a" },
                { "br", "07098305e99db7b7f949c3bc8a5500ecc07a825f7aa68383cf2252ae861a1c2edc3e5c02af474aa776983ee79e0fa0f326e02daba32e57b10e75d40ff5304765" },
                { "bs", "135a6ad8c8a096e84d576a6756d763cd6c6bb6c5874580a2f7202987b93c7cf72d2d2861ca34f890b62a0d3fb9a2a81d9c39c66c61df952928a998ab86b00363" },
                { "ca", "18a3c55cfc61497d62bb198e32e600860bd19804ca8dc7ea73561b4884d43492a67ad046018d15380f7902316b6fdd169bdaa2fd83e0683168bd306903ba0f3c" },
                { "cak", "e08c937e4bfe31fed48f49827feb93824ec1f895f27fce63b63fff104b907a90126298387ec945a9dec96d2eff0a21a2931c76f457befd6f22e903e2eb6e2881" },
                { "cs", "f368ec00dba50a2329867f211477e77d9bfa94dbe418426ed196b669024cf2afee103786acb9b8df62ba071a3050a4c655111386594fa60bb450e0ac2f11c8fa" },
                { "cy", "005a0e44bfff0af8517499d65f162c4ca50932f01bf76bb1a1efa1b53788fe4567e6760262093a666121f1fc72b9dde6ad598069ab86d9df2ec79395d69d0d24" },
                { "da", "4cb4c795b89053ed2e9d07594d709984efdf7a909d6466a917875ddc2a4209f68eee06106811a81c7d506d8b584c5657c5c5f64e8e53128358971c72080ef4e6" },
                { "de", "c7eec89cf3efa9bd95489b1897d4c9203fe3ef959ed72c8fb11ee346f6216e983651e95e8f08c2b68b210acbe8e4d42fd55222183085fd628de66e6efca355ff" },
                { "dsb", "169e28ddf7f65fa15a7d79901b4485ebc14f9f0ad329aaf141a4321cea7797c74d8ddaabe71d147b8e07a1847a3a0312394681dbf9940664f27ce1fc63697645" },
                { "el", "e8ebfbb61b426416c6caf88a7c502e72cc472ed2274baa539a970b24e77c86b89c8e3b6a25942d02f9cc5ce67b7e0c5b68d69871d24c941c5e81a42f2dd95012" },
                { "en-CA", "a96048fdb78b9aa459260ae86147b75a274b56c5ca0b49744b9a438ac14d5a7609aa99b67929bbfd84ce1df4880e839eb9d09dc43fcb3c3ee8769ab60cc0847e" },
                { "en-GB", "71fb93b8436bfa8e1feec6ab95e36f28994a68f415987a9a7031fe49a910123c6b740a33559b73165f9332df7c477fbc4353fb71e8802d64f27d13532d94f1b5" },
                { "en-US", "79ebfc7eb3c449b8d0dba70a7c62b0eb9c77bfaf65827c492554df260f678410b86d0439567ddf2bae940c503fc4fc55150bd0f43db0542fff7092fd2159eb60" },
                { "eo", "44dcf1b7019ec806d79196e75089686716a7e05414f9a5b53ac30d35562509aa8769122ba1e2d6e505b1d6539b53216d4eba589b5bdb2ba581450cbf4d71da27" },
                { "es-AR", "fb6e0fc146f7b2980e6748145ebb687d20e5e1f5f8107ee687049f6fa0bfaef3f4143e2619188615f7cb968c8b1376e7f95a32a088d0fdd58377066ae8186e07" },
                { "es-CL", "291db7ee9432f5964dc95f134f9d6399bab244cd0475d3babb35f8569f1803683c64672e6d850f640abf98765e6941a969d2c62fd6ac7ef9c69e1d525b7f63ca" },
                { "es-ES", "dba41630c02d78f5635f5908d1b7c1982754d9deb3cf2d622a616fb2935b9a2ff2d36383c65648f8cac1e2c15788699cff6be0a040e16a24ab9fba83485e3b85" },
                { "es-MX", "5cf096ead134d283af86945c0bc6b09aae4a72a330bddbc353410b873bb40337c5fa690b7d9cf80b670a540437857c4fd5e8abe5827c04bcb2f8978ae25288fc" },
                { "et", "190a709f25dd9ddd3104c26bad686e3d8741acb4c3977215a2729da280db85dcce9f1e74d31147547ebf210f47f921b73e845a9e8944ce26097fe7f4151fc9f8" },
                { "eu", "59562814ce293e39e333701e1fa7a4c8ac986f2f8d04c3af3bf91c4268fb59db8fd29c388ad5857866cbe44998552f6fed5f6dd7f83260861ca1430b0abe002f" },
                { "fa", "78ce6df22a2368feb64a76a74770c3bedcc6c0973f245f6ab8ec656d70d723ee9574f4a8d13608724da587594356e14e5d3dffdda425abf071bcbccaa0299b80" },
                { "ff", "8ca6809fead4c445d121dc9e9d195601384ee42a1ac39f3addfeb335afe0a9c143f48ab0eed1ce2e36c5da2277b9a7233af5d76646c21b020fba105e4973b0d4" },
                { "fi", "27ee2c0e4e3f2c3850b691ece771b23f6aaa5d73836bf4f02bdc6619963cde4c74b2cc5f16eb8dafee1e66a6218ee63e2fb9033303e0dfff7619471e774c68db" },
                { "fr", "47f50cc34969bdecc8708a0712f56071208663327e7dfeb76e092070e95eed60ed4b3a26f27163b11abcfd9e44e2180ee1a4020247292ffed18314aa63f225d0" },
                { "fur", "aa2b5aa2d6d1462c8c85ac1f312e473e2a77777f44f824b7c45ac8e2ca826b100b482ea0275786d11af48ef9019f089f4f97f62670c692de4cf652165ecf3958" },
                { "fy-NL", "c3e3ff936b332007c1ac92d1a02e969839557706d5751e2febbc3f1f1b84a4e9e93327c6c2dd40e5de945b60cd6ac3eb4191ec375618e5bf2a29de7d03ff437e" },
                { "ga-IE", "d40fc8df582d52a33c73bd9922aa457f9e0dc7457bd23e12b87306bdc37c8976d88498df7e7e16dbea04f4d98dd0137447bc4fb4ee76986d365e3ebf217decab" },
                { "gd", "099c9f97c4b798d3586d708a04b70f9664b11ec073a7e80e7f59f4dd4145c18f3b8eaeda131ad6da82cb644ffbbf340564b7ae7515f0b0da46c35de5c2406603" },
                { "gl", "0eb61f25b62026e0cd35aac90d7d23a0d7a5d8e74cc7ae64bc40ad55728e00eb378a6af5174fc21b4810b72cc2a406a06d185ff78cc4b855ab630608b36fb437" },
                { "gn", "5c54c24f6c023331f18e5db1f728df90c8a31cfcd8d650a39a3f2cc2c9576c6e572cc35b8da547500be40f15d0be255214e679fe9033908f63aca53a7582cf71" },
                { "gu-IN", "94fb99843d228dfe1000a134c825f9e5bfc3fca8f49e666ec62910fb07369866c555e2e8c7fc4549c56ace951218c2d2714cdad3edb823dd1860d08a79b742d3" },
                { "he", "31c042d2b9f9c200d538ae8173f301ff58f38a32aedb85dab519881c7e2407d6e60af69c10c9bc3728a91b57328f79c98302315c235d68cec605c72da316a410" },
                { "hi-IN", "57e4a40ed017b320ec70dd36da0f27ccd6380442fa5c55ffcf7a5af64cffb10052a9bb66465abc01adb9a91fa0275f25eb5a479d826e59ee8c5befe00c260d74" },
                { "hr", "f362c79a04606a07a0b650d68cf884ea4272905cdbf74d668383fd2afcfc531a898a7865f4db3a169f12d75a762e48f8a12cad8161689d60cf60fc9632625b2d" },
                { "hsb", "2deccc50d4c9e66cf967b67fa180cef937dbf84351c454bc190aa2e97a3b293ecb0fceef9241f9d69f532c978bea6f7a6f366f4ef8be32c2e55e55e426a9798b" },
                { "hu", "04fe69ae63a2afc665859b0b776f402438a6e4b341f45d98ec6a1d2f06a35c01e612da49d3ca93322eda79f749c8ab721b3c94435441aedb2bc9fe527495c2bc" },
                { "hy-AM", "f85569236a034585544f9b95d600b0937ce9ff6d07a5fc70312e92e7277279333f9451d0c0af72ea8eaf216e89a0b6275af615e16ac2697e04d26da2760e1d80" },
                { "ia", "62e6a65be8f0d0ef9fea4b2f6d09497ef848c1af2245c5263815711d952fbaa99118de9e1bfc7c31a16778a306edc58025304b1a4fae2b9628ca62a1e17b6ce6" },
                { "id", "9257c40071aefdaa0888e68a3ebac0145377d3d08b745412b9b91ef81fbff05cc878eb644e253363b8a5b214f252f6cf3135622b6ac52a6b6b179fcc92237bfe" },
                { "is", "e930ceea03fb3030206e8ba7e435f8c02c9a695e1d6f497dae1c0ee0e4cb165a171a1be3906b57b607b104a2609ae5364ed08a96a6be4c2e80c895944aa21c09" },
                { "it", "3c6badedf1cbf41e126a100c8d267cd591b339ea35c1644cf3b9fd178a15eec5b34a14462dacb974a49ea1e2f7e38795f36eb179b982d3ef99a8f4a803da253a" },
                { "ja", "ab3fef7fdca3f7ff702291a8ea180753f994b5348b48adeeef3332d73a8630afc2181f73df52b107ec8e54ac0f1d8a0dc24805c7c306a04155665307ef4a3af6" },
                { "ka", "a4ee0b3a9d1c2085148c53b9bb5e31ab2b762b695691196b0b32130db1192ac08a4d1e95c1ee7790c6f51ded745fe5fea347affd58ebac0e354f80513b21b44c" },
                { "kab", "ba136c5add7e91f62f75cb8c5a23243a5127f445b55a4e2a69cedebda0c3edce35984ed8f02ad8f2ac56293675628c18f465d4020c368447c58da98121309879" },
                { "kk", "d97f0780a21a0b8db737825c65ca01c7e9b3258bc0495575cac284135dba0305108b87a9c8bc89177f61a94827a87f95bbe55fdaeccaecafed25ac1eb27bbe5a" },
                { "km", "7e61e2febb4c76ac6e668efe6d8507de6b8c7905edb7ed244f40f6886823f4d6e3999453c17629537705bc0f5a21c63e27867a7b0574ccb45f1da03ce5063df2" },
                { "kn", "c190fee48010da5500de96351dfbde7158970817c8855092664f0b68bb9a290f30118b444fb1efd54abc819bb5bfbb9c1ddec136a7d9a8565892b31c5a6b67ef" },
                { "ko", "4c6aef86090049dc2770aa75b6a0cf0bb722220bc88190363fad8005e948b9685e695cc8b87ac9d2beaeec97f58b8a74dfb659ac601a84b9c89fb7123084c8cd" },
                { "lij", "8ccf9dc8f942abdb3d6887a2e8118b0d2f422091e76e8a121b9c2b7f31b6ac1a07a0bf034e5b0438e9fd6ff560aa693858ae9cfc3502f78958625c6aca53a228" },
                { "lt", "eff43981518261d3ac783dcd7279b012b197c0384b63f44b07121effcf2d3614c227cc6ef211bd89466b07307ab581fa5e8e43d603ef6751cea48ca1c07dacd3" },
                { "lv", "e98673267493dc6df6b178b2576c84758979f4ac0534f07a068990c89c42068b42ced713c68e89a2e2b32e927c185709d8d5c31e022db7f55f80e38e7b9c71c7" },
                { "mk", "6d13982d10d04fe399352c5067e5028c8529f5fc07ed105c9b0034396bcf7d5287788f759fe5db565dde47d302396b0909245cee70d8f06746320386f743f8eb" },
                { "mr", "9594f17beef3c361f94225dbc975870af8eb0389942a110abc4416cb010897e587dfa46fa974358adc91fc409f525edad6d01a23ed2ac53e7d4e7537ab887804" },
                { "ms", "07ed03c028dd387f903e6f73695e3e11a5fdb20df3f64bf1f99530572a18a6ba7d28bede79c1182e3e9fd5020028ded88d4bf2db756c33bfc67900935023bc36" },
                { "my", "f55ae89a33cd34076491da2422c02989d6cf5249a7a2f547de75ce0d61cf261e9f1cf1a4b53e69d798de21f57ae87e0e7af15415ba80f6886582a790e0e95d79" },
                { "nb-NO", "d4bc8a2f9e529202769727724d583b1ab3d39de28998a92d8c6375d67294888b6f960616b74a85c20a7cbc79925adb341d37876a40744fd696eca01eda133c3e" },
                { "ne-NP", "54dc478845f324385183b1ea6be774975596c90c52342f8c8fe61f489afd829513e6964a257db16d82eb776a5e18d362958d1e784cb0c8446cc432f49ed27199" },
                { "nl", "7e4b48057e01d06ddf9a9cc34cd39bc3a4de7995b179389fe862b23e482fa52edb6f34da08745bbe24f96cd6c07e2b59b5800f1c03e62f71de6f42db8c2711bb" },
                { "nn-NO", "eff567fd7bdadbe32ac97ad80097ea15b0a9650cf36d9fb9e633b2b8b602c4f96172705f4f738d5760d90ed0e8cf4f8699e8d0de150bfb4afcb7e0910c9a0c43" },
                { "oc", "f269a162b62b40a4a33d841c9bd8bce1dccb8e9df9962b92bd66831b2209dd72d0f47c7f4322b0da0f5f1ce4035501ecce77820234b4fe2508e9bd84e8919a86" },
                { "pa-IN", "c77778498c9c05a5b6cfe2b7beb547fb5ba49f79edffc50ac39cd44b1c2d2e6af523d5ea15a90056e9ca86879e476767513440da0fca152b2f7cfaa95026a5dd" },
                { "pl", "9b8978b41957f9ae0e1f0e48e3d45876331ae60db68d91ebb666f10afa0f72261f47050aae2f744de5372898b2f95f26559e7864c15e5901a438670bab472532" },
                { "pt-BR", "87d153bf99d86460e9aea55bba79ce45d7c9975529225543d4b602513915fe57c12ff9417bad86b7241e975c8be7008da6268317842c85f5e66a938be2437343" },
                { "pt-PT", "1feaceb5979082d9aee4991fbfe4ad0a07a2bae703e81f6ce15f59cbcf9afc9ef01c159438327bb7e19cde1adb348817dcce70c0ffa2baa543c933beb2d51b3d" },
                { "rm", "d7627cc489598c07ff95e32c6306487554639d838a9a6a3fbb833fff2c595fa42aceddc341d17b9f8166f3b24294e27090645c34e305dac4cb84131b4f54054d" },
                { "ro", "59f8e89af2546ad004a3560cbe75a5972505f7a2f358102cd91fc4789c460ba603779fea2b84b8b9ff078ebbffb6e1be5adf9c2ff2ceabb008c52d10bc128a10" },
                { "ru", "b39a01032d9932e7cc22bd1ec0a4a663ad315f7de630565bd31e307d235d35d8cd5c50f8ecdf91b4f6918c1b48193a33c7d2cfe31b21c544d7050b0447a50879" },
                { "sat", "b59467e4a067b8222a34b60ea706a9bc9ccc22092f541a65865395a3604137d581438ba329283039ecbcff1cbe2db48ed7a95b8031962864b331175fb699ce74" },
                { "sc", "a8bfbffba5acbfbf140a526209769228d1b7abe8def67348288b1dee80cbc3ce7d2c850b62ad75771436d33b2944786c93dd9663f8c04a1ea4a132586e9792e4" },
                { "sco", "1c92a682d6fa1fb73bb36fc674c106775def28227b312f4473b48c9b3b478eb20f4064109f90b337ef3f18d7554a12c17ddac161e2c6af5efd93e5cee6a2c5ae" },
                { "si", "141e54a9e712d346dff1f4ac5ccfa3dadb4bddb900525a3a67bd26d8aeac1aa458a232b99203160bc0f64a0aa549f5f8a0d0df36cb4ee49d65e4571513b22944" },
                { "sk", "ec5ccbccfb60e87cd8dea572f745cbbc92b5100d9b1828b7ec96291911932053a7dbacbd8cd309620199d7f2bc4ea033acaf4f8984edb9bcb7521332d09da127" },
                { "sl", "9f24bdf6dbec4eb8c816fa83c5ef310c46f2bbeaeb9e99d669c880d9bbf223f989a037b39f2e30a6076a390b8ded1cd22b08daed22d11a4d9d6efd6a1530b5ed" },
                { "son", "c37767857f7c844a3d57a2a835ab1e27abe48ff3bdda39cbbaa80c65697a6c02791b984acb6960b91a2d61d39691eb3b20d13c920278231a537f5fb32743140d" },
                { "sq", "45f7f052b39282255e8cb3d28369580efc8e68c8270d581b2b40dbf92c9eccd63cff9fef9bf820ba46d5fe0cacdc100c457f6151a4383187b348f3fcca336a9b" },
                { "sr", "447691559e6a78466b06f043307c6206b5dc9b1948a1aff706c634c6414cd49e07a45d86cbd0f06bc6d20483517816df5c6d58bcd9bde54c24a0b4975d53feec" },
                { "sv-SE", "c245c520afe7f335dcab3342fd4a67d61af10c8f8b9a11c82ad4c0a1e7af3971de989f70c5c676566b84e1fbca86ae98a84f70a6664ec1ea6c453ec62f01dfb6" },
                { "szl", "3344638e9ff732c686a6aea38b261ebbc726871b1524267108c622d6cec6101a3da22e923d9079ccd230f9f69b9fc34b7f72bc940aad295bdf1963f11c823fde" },
                { "ta", "fa66f5b389fde817db7c313d4cd342d3bedde761564e5e418d962ccd45aa9d56fc0bc15c2aea1068657ad3b860e6c1596486ed7fee9f5949c7285293f5d2e202" },
                { "te", "f1beb4621fc1c8b890d110d1409252bb60e8e9fd040634a116f01683b4da152b37331d7bb77cbb85fd39dc4da25bae07d8a3e1f3cf7f90081e9381e673d9d4e7" },
                { "tg", "99e7dcbcc754e400190767d0ee948fc762ba0552c49cc6055e7e1071debcfa77d83afa9b8e9250af1075f81143298a3455b2edc685e2369682728128ceed5504" },
                { "th", "44bd38b604020d0878a9a90088e6209dcd143aba6b68d9bced572e66b24dd6f65fd5a2c266bf685c94defe6423c6c3bf4ce490f6ce6ae7382b2d6e5a84afdb24" },
                { "tl", "54d75636233f46400caa76e326d5f72208bc4fe7a78074649fad929141470137a0958c641c99987bfe74c6c380d149f73b3761710cca37d899702d122b1bab4d" },
                { "tr", "6b9945bd50e15feb4a0293808baa57ee2aa375cb4d52bda987e0b00e55796b4e21a87c84e12d29a59376bc19aa740589082abc8b2a616bfd8d772ed7f96b278c" },
                { "trs", "a152a0c3f15a0db1e4497fd093e8355c3fa98d2140b26b67184738013b293dd085fc02a2d9c376395e81cd5f58a8d965d5820a3a2d8416cb7b2f88e8b8ced666" },
                { "uk", "2884d72d9f737e1aec19d8b79176dbccd74aa1baa2b1fefd237337e386874dc740743979dda48d85d2545a2c4a516c37bfd711cc45cd5c519f4ff8a4a5081d0e" },
                { "ur", "2de17651a02f4dbdcd2588ded804d6e3243ba726c777df3222ff872c95d7b1e227db9ddebaf78d7d9993a01fd7762e4fdd127f3ad2ffbba8917f034f85b93d15" },
                { "uz", "565d79e60767216425b4bae020870456e9157d0bf1f70ee0218c95f550d109ace9c62603929a4371f6b61c4800a1c8e96af3b5f4e248e1e6b19f44db652e3f36" },
                { "vi", "f93c5b728f0387a905298bc0715c24c26a3cbba9587e0ec22d375c0f5fddca0b8075898cb69e57850b0b98d8f421d05d84412cad4ed4b4a9048ba1f454fff165" },
                { "xh", "70720760c4707369ab8141232d5f401563c01372e9c0f3b92f6e41f601a1e05450c19d5c56c34d84309448a1b34c9a2b7245cd5b5706fd4ac047da1caceb596b" },
                { "zh-CN", "e38a2e0dce7f4dc5562704f2741936b5d7137e93f15a394da1065f9bdeed829293ad01099ae7bd47f0fe3d6c73e9bd63a111d6721d8d47504d64082ab74f29ad" },
                { "zh-TW", "ca5f8eeebb9c62cbf31b98ee6f047cc6de68fc10212fdb39b530241356da978f68ce8da4c1ccb3d174adf62f923f9645e3c2a7f70adf807150b22420a31e2cf4" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "eb3a9dea1fc8811cf0fe2858c96643a34a8503a907c5f36590b207c210c51cd7b1b2f34ade993296434200e725f9c288630f9838377e7fda9ebc35c2e9a7ea5a" },
                { "af", "c90d78a2161f6aac03b36946a579fba8a20cf32c90817d5b4eca64f64435f305dcdeb6358b6e906e2235e138751c812b517e10217ccd4b3f7438f1c87484bead" },
                { "an", "394ea0f7b8425330cdfc81d5d70f6ce8166671e7c64668b0e2be714aaceb2641e6c3f4a9fb428b4a963b35edec392630ac57b3583e05494c512b1a7ed733fe16" },
                { "ar", "66ab32807ed9e8a0eadae5abf89ff15daf804aba23e8595e3f8124d3b50a35110402b836bc9064fa9a6d189b3f8fc3e6e7f785329ccbb87099c1a3245c0dbe55" },
                { "ast", "001b7b5a54918f71791b6afa8636ec6c30a3f11b50240763f381c822bd9e84b3968ff36a764b475e3e84439e9ce78f6c398f6ad35c0b45598d56431b111ba39c" },
                { "az", "704c10b2bb0be000f94cbfbddf1e3508b4ab672a3fd94a7a5afbae36e0cc78d0cc63c86508f0a248d58bf2ea1dbfc45c43e5e3d9f972401c42bc15f5579eaa5f" },
                { "be", "6dee14f0f21dfcebf79fb8397523e5d1179deb7c5a2fea71c58ca33728088d83e31c05e2ae9bd09a4465878369362875d4f66cb132431511952657977e950fdd" },
                { "bg", "99eff6e225e133ec90282a430e7ad1c1bdee658a4eafa31e7275759ca44c40733a28a9f17dd686a8d69dd639482498e979c875debd19fb7330988aefe00e3c07" },
                { "bn", "9c55bb714075f7c4f5a543bd11c2ff51bdbd5b67815df65beae817bf644e511e3cdb5642e6d0ba332b984c239db545e2da69fbcb83debc4562d7d4c5ad4add2d" },
                { "br", "e417bc003223c0ae0266c859a5db02129aa91c84682817bc0cd102fed5ac8f8f108ea78afaa35154dc51ac0cb0ac4b182c6cf72d122db961b32fe0ea190640d9" },
                { "bs", "f70642c8ce502b3ffcb7d5b422393e856696a5856c4607b1553664ae01aafb01b7b57c074cc58ddaabdc612b9a91d80dc3bec31de3e918896e37cbe8d0503632" },
                { "ca", "7ffc9eee764e7aca1454ebb8d689c4379510dcf5a5b3b54434981f98edf1690e3c6048a83116b9ad30291d885de80d1c96e4b20975c23fef592736af9bb37cbf" },
                { "cak", "2580c9d1ffc59d5e135dba9096bfa1ce00ff7e39026e83c3f2c1b04a5130888dc6b4953ea691142d5bd10550ccc0da60f5b6d3855504ab8c35c017d5b5fe316a" },
                { "cs", "3280742ccfc9acf08550a0f7dfd9f35249ca735aa8b89a32c862f627b27fd998caab14791c1924a4181b8b4826552d61d4d4243bd76eb70ddf2b6850d928e5ac" },
                { "cy", "10acb3f9970c77c40af43c47d594527b94544ee76c259a37ec0177753ab0cad0623a8578f0704fb9ecd7a0a7451fbb597f232b33fe2693d74354fa5c9522eae3" },
                { "da", "9672dba70235f930551288a812296c3ca7d9c08cab435d941a63adf71264d20b2e7b195d25d58d331965b573f8d3f5e4f7c1d2b8122a4ce19acbe39105fe1700" },
                { "de", "0b6cf26522a7a0a3a2149af70282e1cd3e30ad8f050726ea477d2fd58b943a43595b5bf0061ffb094ade75eae3e119e952445dba0fad73255a6f94e46e8b33a8" },
                { "dsb", "13ab7c4e90512f79fd129d7e73af64a5b78b1888c990f4e8d5b2a0babd30ad2da1de4fc3cc5a2e869846231e4d185befc09d48a5645a1488608d37346d7f2cc4" },
                { "el", "8622b7389c0f7119338a6578f2bb474d1cae9128e570cb36bfd29380d9e8babddb977c89deb54de195ec09bce36b6a8abd1a654109dbbb9f17aed3a9bac69194" },
                { "en-CA", "8c89183c4fb09ee4e5b16b8c45c73f2a02f7630ee277a06ab9e54516caf4c70e9b960d1999d596a52d20b502dd22388fcdfdc2b8423308bfafc576669c56a1db" },
                { "en-GB", "f13cb3c85beb09800410b854a8082c8104c715fbad0246075cd2780086835b5bc38e3b7c65eb37f26c17c8270fbcc9e82fbbfa4e25e4f61c489774882b48fcb6" },
                { "en-US", "8abf9c8fe38685800947a1e3e0644ae6b9440c4951e327761dee0e80fbeaac606171fc6f36c5adbf5f296e764ef44fdc86e2787cd7adc9f4b440b1f7e4dab352" },
                { "eo", "ce81812964f54ebf23dd0775b9b51e61be966ad97d466741d9a58ababc51fb886319508f3f9d304e87a5bded7ea88188192f66869dfd465723ee57b1540a3dce" },
                { "es-AR", "7773ab8542525c7b64c687167b2277a05f37fcb43ded8b59253d42441d138d4bcb72d3e558ff7d827395118732973e1262f0f258b92b5126fdd366aa77a8cf41" },
                { "es-CL", "2feecd79fed2140805362184d7cf52001b15976d7c60d62539ccc036d460910d344b5c8b5ca98a44a1b0fa1c8a9504eb665cc7a3f25b7bf08911ef884d0cd8c5" },
                { "es-ES", "3de9ae9c833b49755721f529658ef26292c4a171e11f29be1fca00fa64976cfc8fffed0c3eb6716e49c8bbd7fbc8f3199c5f4f82133b9a5201ad841016797edd" },
                { "es-MX", "91cadc098e0308b6950800c3de8734c242908996906c83831a70a03c3655d639a547bc2ad35e169cfe0592dd637dd88517363919f70ffb6b289e2ee3501d15ba" },
                { "et", "dbb0c7121f18c3103649992e78c328663a3a1cd07aa8852f54952c9316d7e4e0cdd11ee677abaeb8444db094eccaa524e922b6839a50019369fcb851691e26ed" },
                { "eu", "24dacf927bf35d28deeb178b2bdd7214c8fb049996557ede740bfeb70c50f35d30bc4a86a37686f5f9af4d8679945c4083b518f32c2c318f1d20140cfb70029e" },
                { "fa", "8ef7abf97378806487219e527f6f415ee6d71430c07f03cb48d2a99deadf5fe82474dcd1d3e31e624e1f77eb17cf32b053e408e62b537c7d0bb5cf6ad8688091" },
                { "ff", "8f51a571d7040f30a4b5c7c1072c10f858e5e81fd3a9716922a5f2cf6d5eccd1547023f9f14aa59a119084730fc73315eb10d4ad64ae2b7484cf1fb7e568ac9b" },
                { "fi", "909143080c2bd097ac068182105391888ecb98d44a597d400724edb0752623b540b77976a3e919a7de258dbbcfb73d672eaa9b2b372426f91bd3072a57a7a9f0" },
                { "fr", "60e73d1043a7758132d0727fad9a4c79f33e15702658c47682ed10ce92b9e77e16be07b2499d1f2fb7cd225804ad18f1be72b69cab0b542b996f296e3c1fda26" },
                { "fur", "d36f7f6881cea77c8bbb15323d51e10fd67fb1e19f15509cb2857ee0b51bc2a6e4b67b80c2c168379ebcb5fa5d1f9e923b58b0a857cc56e0fc6481367cb04a09" },
                { "fy-NL", "c5bfd7faf6125d296c91cc089292c392a26df2d43bf180fbfc79de645ad905088a06e7d8ee1758c36a2885bf42d35f29cc7268c3d760e319f802b460ee557283" },
                { "ga-IE", "c9907cdcc843bf250474022272413605ebd1a4c14dc7620ab466e7bd5bfcf31003e3073f977f48991dca325a157be8fed9546daa1f54c4087bcc27739d9c8b30" },
                { "gd", "71af2f5d44e43c81a029e44ee929a15bb37d70e9d685d6cb1862c120b7b09529a74e1ee65233c21fdaa262bb4c0e331932bf811928467cfc659c132dae66f6d8" },
                { "gl", "61e4417f1b1ace7da4a2d397748eb70aa91872c5dde6871b96432e0b01375cd783ae1e7471730f6e3d84ccae9c3dab523536f8cda1850c702afe8b79db30bc9c" },
                { "gn", "8e3c7171c3c5f97a6479da79bc212c91b169790ab3cc75ebb2b2af6ce0e58e8847ef449cc87ee2f6af809910a73bd2c8297d936af81fbd8f2fdb58f052ad55ac" },
                { "gu-IN", "9e057c6ebc05e97fdebd7592acbc32c87cada8cfc68abebc130eda77e4492342c0c8ede5c171f80b9700e2853ba7eb979cc28743df68f4976e8219a7391c135b" },
                { "he", "b8186e8981514b498317cd931b5a28f26d559eadad351f56428ab36e2ea7c3a0f5bb889be88b7c20468315785e4fe5e22afb8c88b08c3c7c5973dd8697c064aa" },
                { "hi-IN", "6226b705e7c4bef4888d2319e0ba52c6c362c5e016ce62532c916d7d96f5721c7df79708b50e0d7e15f11156fc36478c977f08174298fae1fcb80e50b827957d" },
                { "hr", "f527d2992f772a40fcdd4592c2c5a56bd0bf3b2500ef097772d357b0cc93145be56f95dc0223e5ebadea40d0dd09e5f63c4a19463918befb994c112af1355f7e" },
                { "hsb", "166b8d6ee0c8da38126b722b9e4b00d28e3554e82b18beee524c3e5b7ddcae4338ccfd0ee4c40617d765834461a8759f2af80f259ae79e8bd5882ee55b9ce8c0" },
                { "hu", "36476b83e1edfd7b4b239bad65755c4e6f2185480ebf6e5845e0e36d92dd2528daca7da8211b2d5a5d90c70a22887da9c0559b423f8ab3665f61416d2994c9f8" },
                { "hy-AM", "f2aeb11ab7b71a8f647e3d22c1cec4fc692fb25904f1c88fd7bbfa8366334847fe778eaabfab2b9a06792576b71dca1fc180d1e5b94cebc3e39e0ae831251498" },
                { "ia", "4001d7e5fdd4626eb043550c0d872d0a90c0128e1c9d45faec0a0460a32017969228ba8c5c595eacc9b4156d5455e5d982435c5454a675c522f72479c17089d6" },
                { "id", "7f25a261889fae8ef060234c65878948e0a4d689d55d0b528b3900280c9f8ed1f00135538bf4ac65532b9d7b7be9c752d695d09f8f87fd7d60032a89d9c276cd" },
                { "is", "1a8ae99cf3f701f99bd5d7fbe18f3c6813c09ce5bffb95b47bcb4d0892011d01df4682486e4bd35c895a93bf97e47111cd17e5820809f3a1028de1fd5a78f7ae" },
                { "it", "5f85d84b03caf20c00713c40c30837fd9a1f524db2a93469348d36e97afada6361f0f78e6833f00d69167a6283347c4544a4921527eee34715fe6a4bcf1dbf7b" },
                { "ja", "7e59f7608f3f5bc3432b8ac0b4684b57fb3ef73d14b6e2c8eeaca855f427e33bd2839e9cc6539dedd75919e2c7ff9060b16d47192f4ad8e22fb92f5811b8cb5b" },
                { "ka", "90d771a74d3244e595a0e4e16ac91e495f6d8573862ba46c3436fc93c5e750ab82ee702285c46e0b919988af081b80b6c4a44fcac2cbb4164ce7fd1b1cbd3550" },
                { "kab", "f9afd014d1599cb5d4605a8ecfd96b93f12d0c64c08d1c595c14a5bbab9e265f6251fc0977cadb603913d4fcdfb805d8c6ce87262a4754a0ba4713f38b85f180" },
                { "kk", "5a7a252e51e67da9df24b5980c529ae715dc8722ab7e722217b6cce54867c6ae231952590494d148ed5cce64fc06f747dcc8d136fa4f41c29b95eb356958210d" },
                { "km", "a9115c0d667290b068093662b54ffe8ccdde5c12d3f51f9db3e27bbbdd1d0aa3d4e11e1c23dbe6106ccddaa177837930de4c418e630d754ce608da13edf0c239" },
                { "kn", "a201a3e7d1d83ce9a1569ae7b951065c2ca72e772ba9801ccdcd8f25e0acae1b7813b91400ada883f5a8e0c03a87391bafb97b166527a42d10b6add5363d2cb2" },
                { "ko", "54a06c75537579b2312c0ea325b9dcd5482ec481ab0b7dbefcc802c5a44f8d95f8c33a2ebe855add37c9492a828d8ad9c8577ac8bc31d2feb36a8e9cbd57db4a" },
                { "lij", "95c93e224873f2d16bc9b43c680ebd15a8b2979018d2da6d418db76be362a894165ad2509039deed4e00177464ddd4e873fb109a9946bbe16dfcf028d57a2d07" },
                { "lt", "e48d8832a8b41162dfc12fa3d27e14310bad66fef0f8a255a2e0f09fc49045de4e45a4cd9a8941f2144770b5a456c50f7e49b00136f34e30b7ba856a21679b9d" },
                { "lv", "8cf0376b639dfb1bede5221e8c68c316de182955e20c43d5d9f966afbec908b574238be09985f0c0f7975ddd6d7189cb471c1a7c976c2935d3cad93766165517" },
                { "mk", "8eb59ffbea1e6fd13bb049e8c1236449225d80d36276ff7652aa20b8cc07fa2d639149ce6986ab989c7ed702c264a09666ef67e76f405acef6312a721f067153" },
                { "mr", "f4133404d8d648b0d7b98ce37fbdf7e1eccc11f2f03649922cc88aa88995245d46e202b97c67a8ff872d652646675cb44577caa4c78bf6e4a4646462e34df5db" },
                { "ms", "413fde1a7e2b28acbab0bbeba0ae60c101e2e4f6a78a7e251a1216963692a991dfeaab95d6307ca7dec890f93071f485db373a8f7b6fe01c6fe5bde4ae48f4e0" },
                { "my", "ce67e5f88e89448de915bb9bc7181f4222a266ae329fd8b12b1634bc5caf80d7b367965296ee26b70ac20079e3670a727f4a57e98df35272546e9ea5c32c4ad5" },
                { "nb-NO", "e9cf319b8bedd2aa4030e1f6172c8762a6d58160544d9307af837ff5a01c3b906f5830289adda93874fea78120cf71f310021191076cf62ed6e5b02286b66ad3" },
                { "ne-NP", "d2fc64f5360fc88eb33bbe78b705318c24f0138e75db3dc1801a638d52b0901dedb0a603f33d92976801684a53a5ee9c5d9138b9925cc9a3e403e9fe5b4964ac" },
                { "nl", "934eb0b5db7e62782ecca5d8fb0fa98854fd32dcddb5a7cdd5d1ac408672cf2002e22b5fca19a739df5423aacc03c9bc7fd9f595f9371d153e117c80338346ca" },
                { "nn-NO", "297b459af38c6e2ddc2e7900f3f67dfa5ccff070b281a1e4f7f560ddd9e93921e5f102de684f0914180dba4e96fe828fcdc0594808df9e040791179c76fb8445" },
                { "oc", "e607b1d7e912b0bf29dd345e9c8cfbb4b009f3ae8f1d50798476767b76ccfbfc309e534b5e7695ecf6258cb9a83161a297b1bf5bd1d52c0d7736a9bd739c5e0f" },
                { "pa-IN", "8bd7220b101c385bc928c6d96782e32fbe0e8ea903e10139531865895f0101e3df243cd6146bdaf33e532fe427bad84aa701955fd3f08448430e9436f09494dd" },
                { "pl", "6cc0192487d2538e2c36e7ab9a81ccce6fea0dd0054cdc5039b6df92b0a39496c5eeee6c111fe06f1dd2888de84a5c431f6f95d795fe8faf598491cc3e71f24a" },
                { "pt-BR", "5f671c049521c480d361fc257c2fb4a7b7c95a62b49cfceead4421eac9d1096bb1aad97c9bfac0ef48f6d0ca7ce0c30a34776fdc32bdc3c2d773544ff33296d0" },
                { "pt-PT", "627bd21b3284d785b61d4d651696df332c1d4981165eea48c2306ebab58f297b6acaa4c07f92a3eb25f4750397842a4ea61cf7bb228ba6e7918aec8f32956d55" },
                { "rm", "a2c24749be9c0a8d2bf2ee6db9f538caf8863a23e87b4514effba09e7dd044eaa5750541978a0b1522a18492a03af1f37cc429324137c7a98eea6b65122651fd" },
                { "ro", "d0676f414c109cecb347cb19a5530332ba57d2a6f9a84b81e156c5fbfc841916fa9ea814b9236e0a34dbe45733d8006cef5906fced543a3ee76140947945fa77" },
                { "ru", "24200e3eabc5f877cdf5fea5dc2e5abc029400a1806ab2f8ca9670d0fd31d118e650b3c80e64c1578706ce211c0bbf19ce6bf3a4d59e5877601e20a7ea8f7b2d" },
                { "sat", "2b5ffe04158c8b5bf1043c82affbc583377a9a0d566b96b996ac718027d965dc636c6f8f794c00d508a970a69a3f6e775a44f73cc53528401baab7f19c23c91d" },
                { "sc", "ad7e86e3929e08898a528abe8122ac5e51d59e9df5eb5c6924d2b3a7417d2c0bc118230a86900d130877d957ef757fbddca91bc09d6b427c84c79096454f1292" },
                { "sco", "9ba4cade24bf2fdbfd78a3033142ecd648293a2d4b1349ab88802c42fea645825f59afe8d788e416bea180f58012e966d6ac96c30b259b2620bd90ff1e241aa9" },
                { "si", "42779e6bb47900f3914ad4803fa803a36b73bc231c72f45655a283b806629a75030b8dcb04842c4f43193fb38213288e6238c66035ed5aaa405c656349b02569" },
                { "sk", "9823800a1bf848fc5e1890eed9962eeee5741cfefd6a4cdff488a4fd16bd2de1d16c198445069cccc571e5a716f57ff802c767e30f1a7d0ade0b256ca7a67351" },
                { "sl", "7ff8c60718fea91948a098f16f7cc899788b5747ecbac03f3e0419e80ea4f34a1c7eae686b5f8dfc2619507cd8d6e1e178166f0c1ecb39b94759bb8731b3f1fc" },
                { "son", "8535be02bc4c6d6f2f0eb779b950f033d24a3d8d56dbbce5266fefe201543e1b533a91b656234e2543739339fc1ab4d501a7a30220c31f9d08b24055591bbeee" },
                { "sq", "a9705008612641a878c74b09209b3900e91cb39fb76edf08aee09bde93d368ec43da438a84fa06c57ef457978f9967473cd46e995bbb4e67a2749b656148dc7a" },
                { "sr", "d971354d4824f8465b115a62ad7d1a28cf9c9bddfb831e8c6e43332fccbbc3907b4d7cf2c39b8ed9e0dd318f458d62aeeea6eb05db5e299a8cf7c02624bf7bdb" },
                { "sv-SE", "83d4bb31f88b372df407a7ecb61b218795f9ef440bfd5c35a29a66f961c7e74f7ee328f52aa6298fa79672b0e974994cc5453338a616deb906d0265a2812fdaf" },
                { "szl", "e05a6837377ae3bc3046bc52cbf84ba45e52a7dc0b02f9cea36d45974e575b7ea836049f7c78ebbcec6bb77300bead8e599bb19025c9c370234b1223cfb12f08" },
                { "ta", "5dc7f68355c640d9de741090104e4d57235813979acaec3ba548d1ac78c7769dbcb248ae93e30792707a8b82c1d80e97e349d1bf8f5a72a02d4b786d8f9ef878" },
                { "te", "2bed5a70151b14244260ac7de4f523533984ce62640c5df796a302a43a5a35c805df2cce87565f69ae53aa61e9911b5906e84624f5de600f3f3656a7a2f560ec" },
                { "tg", "435f3fc78ede8342f188e237020d2f9671a768333a556544a95404233cb55c95a1913cfb5b02807edba936b5d025f60947c836aa378298be02a7ecaa848baf34" },
                { "th", "bc5b212c7fc4607e80b5236b66be401a1884765e4c4480706487e2c34a11d404f6e98c15acb50b3814e7992d7c84839ca75d955fd741ab658339a0365912ec83" },
                { "tl", "b47447082b26167f1858e83ff58e36c351da073a2e3643cd18d0981db3778c8d02ede3bc98dd5c463d0fa94a6143817f4ae50f969e6e80f6799b6cbdbe7f828a" },
                { "tr", "54f4af02ca837e167145593b95d7472d64dfbf74b846bc9f23d2e34738fb275e4e775172bf9c03ba77c8e5730720e836f7f62d6126d1c89c2ab1f80e47dbe6fe" },
                { "trs", "cc289efe5f3f04e2830b99bed69d5f3f3a7caddc671bc7430d081c5ead640d40144738667e25607a73ad0c2a88a23fb3706452946330cede66920c5a366edc4d" },
                { "uk", "8baeed59f6acbf326f5c93b547f2653a7170e83d90386d5d91a1b86a868875010d3623d4e4de644497204d3fc0ddbe95d30c9e6ad33474c06d83ba1183d20d66" },
                { "ur", "b875da89f664b474cd856c1eb2fa884725298a80b2d44ffdbb4813f43c567600901eca885daa4ebc9d1cc0f3ba38313c406095be9e5737089e9e0a8759aa2f72" },
                { "uz", "e4fb9d9026573b69afc15101114c6a50d3a9a970b9db3ccb18848fea43dae1ce4f3d2ef534592098eff1bed4bd47fb154e2eca9e6df757a632ed89097041664d" },
                { "vi", "5199adaf982bbf23b34d920b466b4e04691dc3d9719b9fd7a27eda8e6e3d96c846cfbe621624220cf5a563c37333e780b06a6be91a955975501137d05627034d" },
                { "xh", "383ef9c091856202e8ee1acd24993eeac36c6bddaa4070f452900ec7876c95949dbf951cef4eb253b42122fa8f2d99c0acad4514820f64ec538ae4adba4c6433" },
                { "zh-CN", "570ef3319486baa2da4e243b7a3f7145e00baea54a12e85ca76748744dfc19c54f6fc4dd963c0dcdda2e0537e5ab98c9be60a63094b0bdd4bdeab5b57242c92d" },
                { "zh-TW", "286fd11220e05461df363d12b519d4c469d0adeb6ff609acf5a191889031fbe4b7c65353b301861ec8f3c39ec0fb86e92f5e2203b1bab914ac30a50f77b69356" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
