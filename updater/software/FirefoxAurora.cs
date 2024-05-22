﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "127.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "fc3958d53b29be34eaa63d9ca47201ceb9720c30f22be3b4a4158ea8dd0f74796dedea53f37d431f541937404de3bcc6f669d25e9c9f44fd5207ce0e8dfe6a3d" },
                { "af", "a90d17fc0915eaf04a64f5cf8e49695b73532dac87f8f68a3eeea2d82c1f35a7301067f16eb6e65ff01dfce7fb56e141c78dbdcd2048a25c556aea97b55c5d70" },
                { "an", "77bc8a6a1999349c86b76be7fbeb716bf59d9a0448db4363c1792939c0545a3b2fc56c08f8bd36e70039fded82346a0e25847ef18dc98902db5e44b1a1fb2a83" },
                { "ar", "31962964ff8c1f0bdd6f07bc59847d8f416c293e4a26e050f22ce0e01b7f978ac701cd028742761521b2f41884a5a26811af15455ce345a3889c8e19b343bd15" },
                { "ast", "62b6794fd3fb684c02f609d6d49fbe9a189fc32f6d4a243cb29561bddff908c1ae5297baedacbb7b679568ddd9caaa59a441069a421a830ad54941208a74b3c8" },
                { "az", "f97174b9a37db0d3751d6290a1a1d3e1b4af42ca08b211c490bb3c65d5c0cba6e7139338ddc44e5f3a2654f4414306d6768d69488aec60a828977b6fa7907060" },
                { "be", "baeb20177a5d5385778e9d1ad03bac21c60fb1f222c5e61ba94b639dae9c5edb8dab863bc84d656434a9fc165a9b197c22558f12732dd290e2992daeeaafe29d" },
                { "bg", "23c79b58c2b46dcfaf9172069fa8a44c0b9eedbb8fac60c4ae7c94b93efd47c2b198f9b11c591a429cc9042b2f2a4a811af9e66e4f5dfa0e3269aba3fc018ff2" },
                { "bn", "3ba6634e7bf2ea5be63723d0ff6ff4ad2dcdc075e2f7f5e51f63e2f0641900ace18c256dcf444570a76d21a02e8b2b59a1ac2f72529d0159217a4fd37f62d26a" },
                { "br", "e1a68867f5b20e3d1cad5b6400270f1aa6827d2cceb859a6587870b7b480ce1faa0c7fde40da195a297b7c95eba9bf1801c50bf0008e60aafffdb3b86360abd1" },
                { "bs", "929eefee1030147c6652fa45e730bd7e48e57be543c53cb39e31b4ddb8628f01583b2a52bd9a8347490feb8273438a0b6c6caa3742719b57b048604e1049b0a2" },
                { "ca", "e7dd624bf9998daa1bcfe4e0d5de5ae2ac7c951ffa9bbb972025713a678a67a2287af73d99f5912843d6fa5864230a077867c5b47f82037fd246b1e73d8539a4" },
                { "cak", "b03d2eecedb82f3af84221bc6b24af8652885b7489ce3dfe4d1e19dd1b9f13867841b1aeaaed8d6844b705e77c96c92c60a8b1abe9a013c2d74a1160d9433314" },
                { "cs", "8318e56a15baefb1754f17e954dd46822d1608f99e06d8bbcb326bfff305b1d8d2ccb968ecb71d00e39b1836845c00ac48b78aa6bc9fce94a7c12c45a3ebe04a" },
                { "cy", "3ce32894fbd2a490042ede38113f885140b2458306c26d89c76d2581ddca979930d9450f4136577fa2d0bb5d28665bd715d47338e17756366a97c82d91903451" },
                { "da", "67b8fa5cd10e2e26ff4cc5bbf576b10aa6ecfbf344014fd036d5d8a3d2d4cdbe4c98e251eec354e2d7d0af4b86d756f75d847da3bd19193165a465fc7c51c941" },
                { "de", "614a73ace6942c3e3751af0635fdf4704c9626995fe49fbb338f48af85113a9a6eb1d55f4f09ff0d104e8cb8833a439629a51e7238ecdfa481c8187088a2ceff" },
                { "dsb", "96d97e691b41f88245092bd42c51c1c2fa81068afe2e952a81ad77f345b7e94b62eb69473ad5f32529085bb88b8f576515ae63656d06be93976bbf976a396905" },
                { "el", "55268234176c7f623dafa59bef17280257ad5adb85387ea061fd682e4dd931a49c1ae42bea87d94297ec48073f3dcacbc5976f6fd8ac53022497da31c3a1f6da" },
                { "en-CA", "0fa06b86b86c7106fa0b0f1faf4dfd2a4d9d2baa94e39a1f3146978f2cbde6f9c48cf171e94b8279252177fbdc32052e39b8eaa5413724aedec1d8cd7a418a02" },
                { "en-GB", "4b0c76af07d22c3c4a71c3856119ff579728c91986650c86f401f0b611ed0ea4d4607e252125f426cecb51e6f8a7920300d8b2a4b65cb1c616b2e59bf94aa241" },
                { "en-US", "f9ae029e21d5db387cad5895b820b6d54c00441ac3384b3d41d9168bfa16edb2a5dcb244e2f56c3a1b661f3076f176eef6613b53dedcc4fc869a9a3a9da90d2d" },
                { "eo", "7fa4d7ecaabce3b1af430f7ea70737a27625d4be970a64c1e5526987b5deb781c55942c1262051dcdd6ba42227df183f330791fb00775f7027fee8af0d49a812" },
                { "es-AR", "109809c8a31250e64ebd57cf2c723a7e6771de99af32fbc6a3259d767423605a3885f9f3cbf50378d0301b395df7e9a6ebf92775c7a254be0a639a20e177531d" },
                { "es-CL", "1fe8b5939f575766da8f341c9bfb2d092f4b7d60dafc8db41583f9e156c2a57ce67540cf79aad2b91909446c6eb6264096008d8ef2e9702e263044efcfea93e3" },
                { "es-ES", "03383e285d23624f0d0af1d92ca3bcd8cf315b2468d43b6cd3f976dd1469656365a3d8e7963473547b7c49fb1690137b5288ce0b419b4e6a96650081b7f7ad95" },
                { "es-MX", "0b79fa472e5494293e281bc75eead291a59a6a9ce178f95826522a599e5f1f5be2b33201ce474887af77493f48b1a3f14210a5d00670935cbf391e81f8a29c19" },
                { "et", "9278fcad1e913472569f277a4fa0db3f6e12bfeb216c97d71daa70bed1aad4f0684c96dc3fa2466eb7ee815c0997973c5f70c5fe56462701fe816cd110b74233" },
                { "eu", "e55f4048c876a5b6bc629f573ef166934ed35a2ea00eeb48ab23271bf7f36cab4e2c7e111f9b175e42a37f559abe02c979f653d4a209c9ff5f8e7ad71fb712b1" },
                { "fa", "a1eb2176bf637219025b14ef8aad40d0c93f56f5023f3cba0418b3d75580521af813cfbb1ef1ea5d10527b4ab0f95e1dc9d46f8d2a57662068d3e9cf891a8cf3" },
                { "ff", "bb8fbf12a514c7329520fd128fc15b657a134cecfcadea94615a60f38285ac83e5577180fabdd3a05cb3ce394c692f3a0c1ed245cc9a76d5adae61086dfe99da" },
                { "fi", "3d39f5eb8e7e5f6760b46a33afd8637ec56acfecff63db09b071814f30c7229ca7bc275082e5c509fa1eca7e074ba802c2b96828650888914987d67c18eecb2c" },
                { "fr", "850727a8e630ee7d4103556cadbc9fe42c56d5c4a917a420eaabd0ffc32520147899563b99a8e5278de50fefc39ecc330e3f9c09a111475981e1763be56be391" },
                { "fur", "42e7c8f3a05e3dd18cb5ffc5d325b477df3256cfae33638810d0da3376d996b1dc19ff807d431b1e64ee504407265fa376e3e345169c27d1b9179a1f7d623fca" },
                { "fy-NL", "0fa6e08ff929886af30cd42e001473b26e5a0f56cb114236f000a001453c132d421ec608fbbb4f51d77081e0b6bbe02b0783a35b3351b9193490ca7b300ee49d" },
                { "ga-IE", "ac20ec511c5a05ca260aa1215d5eee5d74d8fa92e9588d1cb5c99f27ce7380c3ca135781cb03347ce6546ac569ec06e40291310377fa32f6e14326961482989b" },
                { "gd", "0aab73eff0742e25b591007c8d41df01160037b5da07e24ccb158e95ccd2431bb8d2f9a39284e9cac20402c032dd465808d5cc1b7dfadd62bc1f7bb3b356e5cf" },
                { "gl", "2d2dcf0c082c7ab102bf7dd271a34aff05d35b42c95edd2ee58345a26053dbde260cc4ffb2dc5d8706318b16f064c4bb244f845dd24ee27bf33e5d71fa5a1b94" },
                { "gn", "ac8a24b17eda374d38ad84905d87c43cdb19cb76280671cb5d0425249c106079ee587a6c44d71ca12f2b6f5624d49296ac7d2ad2fc7fc1a72a0c93b68b9620ed" },
                { "gu-IN", "b2e3977ad73d78a2c97ffde0830accc1fba60f90c5fdc6788164e3760eda0c7f710d204ede7a4630b130025db3a6fd8b747b49cc3c95bfa7765f636bfc79fb59" },
                { "he", "e8cb032a302a1958285e33dc0f320fa9bfabcd6ff9f1d75868e6cf9d5dfaf33c38e7b236bb78156725db0925addb00a8adefaa7143fd17dadceac6d775274000" },
                { "hi-IN", "ebc91c57322c8fa09caea5816cf8cc51a92b05d3787ca075f2dcff2ba86b8a02885b36677e79e0ecb37629510bd4e11e7d34f07614721fb35bf4b51cf07e6b01" },
                { "hr", "2a8ce5fd118b1a69f0fc1f55564566c1c0d19122e4460724d0f0a830e0f81ec9cee48f74d4d9bee7a7f72c03197eabd7d303849c59673943652e78acabea7d4e" },
                { "hsb", "503413a75c506e3529af0bbdc853b2072182780b0f61c48d463b9e8e61415cc7f6a5614a86be454cb4b540080f0d5673c4d14a5c11f211010dc501d19feb0d85" },
                { "hu", "f04ec285a862fe75415553f2da3e32da3d65062c7fafbd7692fce17067bfa4168a779ddea2e48684282d98402b492a13831fa24eaccb86e838e89ad108b7eb99" },
                { "hy-AM", "3be17bf3a9d6675c5b2ac5a096114c159360a2c76be172adcbd78b3a05e472fd4647c220e6ad7461ab813030026d59f51efce6d124e8e38aed9b639a0ebb550b" },
                { "ia", "349f5ab0799a4a70ae24bd2ab5fcc5c2eb29dd234786693a6684dc7009e29bdb41e417f6d397b0d21f5c5ad81117153a0b72ba24cd5375d05f8a2547427bc9f8" },
                { "id", "bac8edf1fc19b166bff6fb3b33dcd089a06df15f08b2a373361c3626909b24946e7926e66b462126a28fe0d2db3c8f171f6cb033036623847a6506eeb62e2490" },
                { "is", "92b995d23f661f53397b50d4beb320180f18bd6cdf97b54169e08de81224bab7215c6cd7c415605de1f7af6d3b70d4b7528b522b4737c65ff134d8006ffe47f4" },
                { "it", "c9c7a69267e1143e0f185db7c37888a74be9b9dbff91d531ba0354046a9a55d1a57d44f6b42fa2538a2bcb65b358ad1a9c4bc370d124e1211f2376089fbbb02d" },
                { "ja", "a7b28c41a76de15d8ae24f2e2a86a04c58f043cf51f56282bf8dc8b0914be96c3dde9c4dd2ef4e8898bcf6743e8d0b63a84b8c662479f0b32ae75f4a83164ca2" },
                { "ka", "4b383621911c4e89697b821e5ca99c6b18b7b52344e2349a8cdd330187ee05e5cfce405516082395cfdc9c91f9c58c430a3616b223524910a2f37c9ee657d5df" },
                { "kab", "fa8653151d99dabe63fdb1747e2ddff7c64b6b702a9d1257c932cc91410f3676d4cd620dafa5f755f72c2370f99f2c70f430bc8f86aaa27336d3160dd35e4f63" },
                { "kk", "5db4b09904a773f1e1eea3a7604497ffbed145aeefd8cd7109aa1d27124f7e642ff78adf68bb77bea1561673251e0532f09df3c49bc32df05f3c82225a624f60" },
                { "km", "551de8022a22d78364b89c679bf1c2c9a1d9c3050750ffb99b9169885e3225cb2d4879702cee5703211033f2c3381f7dc8cb23eff029afeeae665eb5a2345ec3" },
                { "kn", "70bbe6bed10813973b02f96b1724d80881167f9048ac29a6e5e1e329ea6a11420f35d78fe890f1ef27ba6f1c05a35f7980a18e7141df098355e74d1a839186ab" },
                { "ko", "055435f7e3fe72b0e3cbc2c604614001ef0ed75f4243ad048fa93185d7b970ab669712fc418659d6ba282b5711f9e15adb07db520bb029cd087b849a833f7f28" },
                { "lij", "c4bea3a4f71b963dd1c16af3f051abec24b8ee125d9d3d9d45d1e964aa4fc0487e7c92ce127bae78f74b50ec55f73640eeff827168f8a90f462106c7cb1ce6f5" },
                { "lt", "18d023987741ec618b8f9e6bdc1dfe70d9d95b2acb80635d3e46c93238980c442e0eceb25276df3ff20f697e5e6d59d322a1fdd8edcebed1990535037651cd87" },
                { "lv", "0ff99bc19ae5b2d44399bb477d2560c8e0436bec800d10e5abb4f7948828e80a7eedd366e0ce917c3b0ebb1cd106625ef57cd2f0184e967a46a6634cb0d8cc51" },
                { "mk", "43e5dba8a969991f97a09ca144074d0824651500182ab9350de173937670833031640cec4889af0ed719b6446ffc76c9f8b735686f2c89c5a97d283ad388ae27" },
                { "mr", "50abfcccbdd63b60407b0caa74055293e2a80fca4907fb933ace7fb3b0645f1451abd2c30466a3b1e72c6d8a3c2842b18719ebba201adad60b5a087988065958" },
                { "ms", "fadae8578e78effca5fdebf2dc64b8e55cba23a6de7798b351f68ba8cd38895132b4b5b88e6e44be2c3fc776771d37c5542c95456147f1777f4419f771c9f25f" },
                { "my", "29262c7304d06359bb321f21b1a2ee0de1c951b4df3ba4ad576395e51230f18bedc56a2bed64f9a8fee4b6605dbbec6b76709a2d84750f7cfcbdf350e3b53345" },
                { "nb-NO", "70cef7286bbcbed6000f0167ff7bde2f62af22b06cc7ee259800e8d4a50d493a702a89a1499bcffb946a6fd40c11a956725f5f5616c088d81b0cd79b402567b2" },
                { "ne-NP", "c1d3f56ca8b28e4cb24bf0b9f44125526e0f59557938f633335d06e97b4de6b2553efd392aef8d0fd48d75fb45e2a14e2081f52399f5c0fad95588f3a9168451" },
                { "nl", "17ae8fd372e701726a200e4bfba4caaa710877596a48e5dbe063accc5c3ad953c05dd6fd5b9afa56cfa9f95774009392ce5457cf7d3cdb0dd13fc57bbba964ae" },
                { "nn-NO", "14d27fa57e8869c22071f4780cbb14119601482e413fe041c8899c8745214a24670e9b715db69babad6e8e5c8165708e0fe17ad27aed1f50841ce7082a5fb953" },
                { "oc", "ef705d3540aa549db7fc8043885a39296a05a4e38bd53224b996c532b1e5a5ade8e47bfc19cc01c6931a4b89b15a7dd14508a31306b52e7cea6678fcdd88d6b5" },
                { "pa-IN", "01fa8bdc5f0767cd1bf457811b398f07bcb4b682e9d8c01fdb51e5268a396b8590bd10e662bda99b8fed17009af8bce1fcd7e6a63365997f71b04871054d7ab9" },
                { "pl", "59882a709e0a6ddc326cec70584b2cf292a59b4ca735c56efc83d2aeb282d7c812ed20fa6723c75c3ab11c9b0c310c1e87ec344763f6b20077ee916d539fc633" },
                { "pt-BR", "69150e29380f216fb7d3576645c1673f8ee8abeeaa2ab9dd5cdaf2b5e2d3537737f687abcb151415084522325976d9b17ba8139ec83d36a694fbe5a1cb474d16" },
                { "pt-PT", "389c1dd6ff44e4efe47113a9dfa7ac3566946ecb967b5ad580a7c780e2f0efd5ca830384cdb4c5a85dde33cdceb6c64f382748ba90b51544b378cae07b5af4f5" },
                { "rm", "0a5e4eb913e921f4de1d5852ec0f2cea360a802712b2e2c484ca616101a82867d74f967c030a39b022081ec7c744f1a2de0a6f624bb1007ccd74b05570951d9b" },
                { "ro", "14a211c5d5c6f06733318b7dfa2c673548303d2c45c5b27ed6a18665d9a83bbdadede17771fa63769dcaaea0a066c9142fd0ed34d0f53c672338fb611019708a" },
                { "ru", "c1f5c8fe884cc99555907f05b243215bd550b3430de18de1d0302fe328bada4998c1bfa8a1153616e5b1e5b11225aee1f6adb32cd6f0c75a85cc28e7ae05eb3e" },
                { "sat", "8ed2b2a91ebea4fa1d04914f4fb3cccd147249fbda3fb9913fa25320e767f813acf4afe6d315114d4d883992b28b17b02d412bcf0e8758aba3ade19f389861c3" },
                { "sc", "b69da54149674bcf2767d88ce920c9efa0aa937b433fc4f75ffe361134b9d8530540c74e85da25cdda31cf8b14d9458b7d6d232590f4f24e832ca9c9792ff495" },
                { "sco", "290dce6eca769702ac11d57329ccdd574d75ed4b025bab31014f865b303e7b461e1f39d91463accd8becb7d2fcab67ffdc6433993ee3dac65d902e3c38f333a8" },
                { "si", "69f5b4672ff51d835164f95a94b779d8ff0b326e59768540606b8e70c59a1ac6c519280bc9ba9eb92d275d024760ceb67c9c039b6e81af37aac2d5734f5909c0" },
                { "sk", "8b767e5b0dca6a5ccb7e21b446358ed451bcedbd0910721950a9fc4ecf7a3faf32d5333ca573e3a6c0a7af6672177fbf2d15b5a0f20c4500ee7483d5a66305f7" },
                { "sl", "f3e621306572686a453aa623b85e4003af9e54fb4ddb6c9df6097b6525124a463b12db027e08dc8e998c16aa48357d272adef4236cf24f27f12fd395fb092e85" },
                { "son", "4fe953f9ad4a2ba76fb01c34fdcbd0996ec70f19d58ca241fcff8ff5ce2b14f478710186c42f08a619c68d3dbc33d8c597611f37c5da32c3f6c2e475a84fe0e3" },
                { "sq", "f526f169318d85e737c3fbc6ae58c5b641423c70efc0f7fbff411ab98ccb873275bf6481577b663a3d39821a7791cd199e7ff7f156deff134220e440bbc5236f" },
                { "sr", "2af0419978423f4ad48ae9dbb3e3d0a50894b0bffc1fd6b1efcafbe8a1a58e1c690af6a884e7167e67887d7c03da618f030aa40d5376fcda047cd08819417fe6" },
                { "sv-SE", "0fafa52cb3915a3db83a93475ba7cce05ec1784627bbcb5dffa6eb21c4282dfbb01adef965f4f235311813d03c55c5eaf632a7b62105d1878bf3a7962900728e" },
                { "szl", "ed99489ba046b6586a2af6288c36153bec0e3d818d0abdcdedce601250607c07bd87f1b37a661c12c12d8dd1d789ac6c29ebeef47b3e08303bf23e22cd05741c" },
                { "ta", "f660658f410c8cc07a0febb54b487c7836f79fcdbc137dcf0bbbfbf8141925032b24022c62c46dd6985421619117b3f9e68dc0350f7564cc4de0112c7d949336" },
                { "te", "71e3f97e23928f6a7d917da35bb851d636c2096f4382932930f8f674cb6302d75177dd2102884a350341a2ace862e697479dd2414d2d41e1272126c53e5edc0b" },
                { "tg", "5fe507210e2db695948e11b6aec20183a7707fb33bec1e8270cf6a7c30c4d639603a613e9472b81e31ce37f75323c58b1f98d3a87bdcefc12d8868c63e7ade4d" },
                { "th", "b78c45ed134038b3481e4dd752c41ddb33121fde27d38addb07adf20aa4031feb435c8677570d3517cbd8b502a02f5058620408c37a699930c0460969caa71f8" },
                { "tl", "801efe3b21aa747c10fd769fda7b855f9a37faf720d0609f614506ad0417a496e6de5661dcf698938cb40c79a49e633a03a776ba99822c8a715b5781a1ca5a54" },
                { "tr", "0b1bae01b7249ee0a6f50055b3d492340d6349ce714ba981fecc9ef95a928905face868ee7fc2531bdbc741962e109056356de131dfbeaeaa3f8a218df205027" },
                { "trs", "4ebfda325b56e8c1d2e405c1e1571337df547efa02e5911a448e5d093089e617b5ae73d3d6c71748d0be79194bd2d4a6363eb83068da2a99797e166780b057dc" },
                { "uk", "83dfe08c1dc0b5649a8e74d59f3161241dd56e32ffbe08e7f15607eb8b7cf114726787d62513763dfcfd93e0485880d4e9c0cc0c52a1ddbb9051992e3a5b80e5" },
                { "ur", "ca4207d7c01627a5dfdd353f44f0db80f4dd42c744ce4b7997e4e70861c2552edc2cf0cd0669dbffbbf33025522f3f680224a3530ab7a3da5d9e926dd47324f6" },
                { "uz", "616d4717d661a00b34ede0f16c63b0263a030cfabb0789bdd3183dc5655d3cbb4d2b8d34c567d5cf23ac046b7d83c3d078635565a3f1b1af07ece7e630b9fe05" },
                { "vi", "72db85f783e0e043722ed958c1aaadf2ef58ac30056e3f2c8ec5b188984757f78097282f6520cfb75b662f6d2018c04f73bf857dfdc67be8f1df9ab3c66839ad" },
                { "xh", "0268fdf12172590770f2ea430662232373f7464aa0e1de3803adc3d6262af0ffec74bf646467f2bc5e575f7e7beb9fd8977e4fac2256468ec62f2ced18722997" },
                { "zh-CN", "1648b1acd962daf9fd2ad7d95f2da631df9f25651246249117c703e01fdf3392913ac3ad519e27ef969be334abee70a832c1273077a66f444c1992440bb40bb4" },
                { "zh-TW", "34da448f330128861718fc79f2411c8b8673b22d06c01a67054672203b2209310f149104453a8ea48d5300ab5814a6ca376d6286508aa4d634ec2f8d907e1a5e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "407b9f4c5564e6c6a3e6eab0f4a8002c8f8015ac73e973a9d7752807a51dd9485f4df3740d2fc5215294b0199ee0418a98fda159508a2e6e34a0b81c0a79ff78" },
                { "af", "a703fb848754a3582ffe22795fc0e549a15935c0c4210270da226d972eba23ea545c0176e66c5a04a0619ff1ca14dc6f922800e6f3527e32fe77da5a40c33496" },
                { "an", "7aa046c3ed9083936bb5402967c13242d9b21c0a55ff44fdfbc9c5df0e6512aed9335301a7ed1cb77e26c7eb0d3831e6d00f3a796ebe39eedba37e6f29985587" },
                { "ar", "fc7d60116c72ae920999a837fd3dda53ea35015f12fe213f3cb6965d9f7eef5dcbe8c73d66a8d857bbffddee8f9c6454806210758057dd1329870b34c14e8618" },
                { "ast", "3760fc3c49516ee1fa6cc3450f49f542bc138e68d85f9218954df8e2e7c628ba4dd9c266830723c23fdb42a8040ff605c6b952c3484e414e3d6119acced78865" },
                { "az", "f2a2036e8512132f0cf4e9306c318c1e802eb583f573012fa96d3c1d8e2c139f8dcccb2fac6fb4daf05a9b16132380ef6188beae011e2f9028b14b11d9b3338e" },
                { "be", "addfa29b40936aa5d67a09fe7f7efff9974d835538eb02e37d03c67dce48a8217c88cec7844da986578fd9af42dd1c1006838b3d8f33d12bfc329e7e84594e25" },
                { "bg", "804015252a2a5281851ec7346c50531fe4179d00cea3e50da1f43d2184868aff5a0785a2c44845cfd3960c578321c4a2b59d97ca8ae164afd4d8bec3e5d1f38e" },
                { "bn", "6490296f46507342b29856a4e482fd22dd2a3b175afb83ef3b00ee04e8054679cb5650a4c2c3b543e5bfa2b1fe60e248ca2d97f8cc8f1f9e4abc5a8863084a97" },
                { "br", "20b926356c9e90ee5fb334b25aaf8a5978f169e506865c0d2155cdabdd4c4cedcd4529b9e13708110e035bec9231ce19214f156df4054d4497d3041c01857148" },
                { "bs", "a09bc3e56224497a36f80d0f84154ae07c3cb864a945725e550be5ce40f8cc9d46c6a720d50953ba3ebcecbcbffab1f7bd3b98836ce51b871a55049487137503" },
                { "ca", "e50d5ddb084350f77cbabb0311d2651137dc4e681f86e67f90929680e6d8621a074cdbd31908e30ce27d24c8742167c9e5074dedc856eeca68edf1e38753ecdf" },
                { "cak", "3ce570c680caced4f923f189966682275ffe7861e27f634b2b62b30d213721ca1ad04b731bd63ac05ac5259e281eb2cdbe44c1ba0c576cba923f4e17279f2ec2" },
                { "cs", "bd362fab0f49d94e4ac6374f9475607d0cab05f41e32a1468aadf9880af84051236679b2dd1da5daf86b47bf3425143c19f9db3b0316717c498eb21465eab3a4" },
                { "cy", "485eab305d46a9ac70a5e2c5aa5231f8913839983ca1a2a1e2ac7f8125bf567d8eb8d30e7ef19481f1d9ca0dec33e52b71703b17260dc41124eecbbeee394698" },
                { "da", "57d91ae206d658f1b797991bac4337714313ecc9d81979ec97b2b23b9c00c4daca0c6219d4f583c88242550808cf7f97aeab7153ca3b393caef2293d0783fa35" },
                { "de", "2e78ea8ecc1cdda4cac87b2a12a032a7e0f5fb1d719e3dcbdfa75924516775f5ba845b7a459bcda62319e4bb624591db9ec4efc152ef12ea4a2e80887d9a3bd4" },
                { "dsb", "9a4e22a5e4025b18a3466fc733a8f757f2023dfb21abf94c268b12e3360d18facf3e60ab74618878a45ca4760c9316998d9f2b25fdff83665a6cdfaa1608b50c" },
                { "el", "388114b9e163e41ee82099718978f5b578af0d79b79df77516e576fa4b97f7b540762cf14c07dcf7563b84ea897c82d716d68b2b32a224f88c0e5ef275625cbd" },
                { "en-CA", "e836bea29153785530d439ea6af856541f40c98f49729ee86be88d0ad09c8b1981bf1532fbb2e88ac3a0cb938744644acaed0f59f504534fb7af67e7c6b4fc15" },
                { "en-GB", "564f77822413656142a141ac079d85427ace316ca22f06726a0af86d4b089ce3017553f09055c10af1b0a191e929fdb09857f664ff69b712771e84c4b4253890" },
                { "en-US", "36a6b54a72479e33d5dc9a54f012e871856bb0f8ba2170353fe8914acbcd7616ec33949b90044918a3f75674dac7d5d0d64f2df9ad0f32f4228466a606faee78" },
                { "eo", "ac0db5fb5958ae937ed53d8a8112b540b393e6a7b46fbdbe20d551c4830dc3ce26c8f6f62d90741ec8486dca3e3cd90782918b83a1f573d8161eb2cb7c7e6d23" },
                { "es-AR", "5860dfb523944e0a772fa1d2ad403ff018442a343314feafa520fec8d89d83efef152a8206d4395cc50a27f5131db45e9710ad2dc4a87489d945a3833767caaa" },
                { "es-CL", "896bb708b09cbc934ce75e3b0a01bc7e6766e1d31e9f0ceb6ae22e8e4e3dffd5be4417053165d3d50ac0307780f45b420446b180cb192744dccdffebc2d42f7b" },
                { "es-ES", "abb855525b01d5186b1689017ebb597c74beab1f1cf89c5f7e9756c172ff4bf5fcd75707ad982df28ed590fb40c4860bd459ce5bc7da53a623406119772dc1be" },
                { "es-MX", "e1a59fc676f782a2c811cf399bd03d9530655b7c6f838fb0d06137e929b9d8f8ffca45cd1d1fd9967c416545b1f9a683879dc5c540e74d4809c453808bf27964" },
                { "et", "5f2ec3162222eb736e461ad21e6529483f7b7316fffeb7be730ce678bdcd4363f24d6e1f52c06adaaf080d540b5cbae36403b89d1ac6982e5a15d4a2dffa2a23" },
                { "eu", "0a99b969d53a8f1cc53bd6472890439bb56d1d40a123b561306423fc33a9bbd0f2114ff264acd5f215a0ef911085873a513dbd6674a1218b4c136a4e22595bd7" },
                { "fa", "77ba54959df6d8a4fd195e114d1bca42d0390d10082f805edba8b09599276f842e58ec9bd98191f5bffed98f48035d8b9848848521351160d74076088e773101" },
                { "ff", "011ccb4e3233dd2eadfed1d5628a3e35396fa8f22185e2cb2f6dc69038cc42369c6546d2ef3bb417faf623506b1136bf7c0d41d92e5b2827ce055b105b8043a8" },
                { "fi", "6b1e053b7a67a63d50a5a343af50e064b15da8582fcf70bd2dd7f498f3805261b4d6094ece7ec8cf637076f244917b781e8c7a425e472bddced39e5956c5949b" },
                { "fr", "54829e2c5c16b632d13a9a838d4964e78f37d8ec0db477314d39e151091ad07d774bb7d78846c350f8247ce31f05373377244df291a4f8959f67145504374691" },
                { "fur", "56b98be6764d92416497af3e343a0260fe125b05ad50d43f9e3571952d1b17bacd68cab6e27c477529813f2f5aa89a115e698be35e7279f8c89b541a0b907a05" },
                { "fy-NL", "4d05f6ebd085dca2cab372fb52d37ee47a1d02f0fae5dd660bd819c0a4c3ecd4c842d4e974d3a135aee199fbcca2a3f25a304b603b6a8c75a9300517037e474c" },
                { "ga-IE", "7d9124a5b2a139cfdcbbf8342286728ad212f73e211923c7ac8634e0f4f76c27871dff9b17bf209fe1583cd8097c5391da7b8c49b48986398da6d888aebab4ae" },
                { "gd", "c36b19d95ea0c997729fec42aa9ea3e1e7eeaefa5dfdcef99d24b99ae86b7b5ef1c34ce6d1471541fba031f8609114605e8c0e9d3b85e65e866b0a8a4af39214" },
                { "gl", "87ba3aca11644db3e698134fa29970abd195c99c045a572ea500dcf3a31263fda1c65cc7dbf553deb63cada377dce4c6ad21af63e5243c17b666d1581ffb53ad" },
                { "gn", "00ff427d99fc70bcc2b9e086697e6e5ab2f0bde84606e1308758bf6b602aa3b045eef89ec4f6aa1a9bf9202c2ce1bbcd06cc7fbd0b71a1c9bb8003f1bca9b088" },
                { "gu-IN", "3db74aa36b4d96efca8b8805d159380ea70ae7fd4a5f3645a0c480b8a0cf4fb01a930b398fa04be8c486029e4281714cbaf44d25cb3ee1fa0ca2ef69ddd073a9" },
                { "he", "5512a0f53efcbdd8b20d7327f26fd48df0786b049ff689e28b90d34ff3c5e619f84e581750f1727e1de011e531a2e2642b0fc3fafbcf1bf48d58473185c849c9" },
                { "hi-IN", "fc6d4ad2ae9c001440dcefded75ecfc852c86b8e0e858fec437181eef8d099b752deb5282305bb1a6e9195f767c700f08ed504e8c151290a9ff39f806737c398" },
                { "hr", "b256e4cf72b2aff8f304665db793909f86e5e7fdf1be83604d03cdff6341e0f4334eb5222372a5acaee4a1a06e6c9725aa89b522e60dd982332dc39a2f98cf42" },
                { "hsb", "af4f8d34732d1c9b7ef077cfcf2bb9d99ba488be2a991e27135157a7d954fc71c7ecaccd6f611e003beb8ac7aa0de75de306c9422d12df65bbf1dbcf93dc5d3e" },
                { "hu", "bac3be2384ef92f906bd7021a292c5c7b01f1571b3b478d9e15047b03b775a7b95678056d5cdb2cf5af4d28e87e7084dc0fe0c2094cc3f66d89853227ca6bbdd" },
                { "hy-AM", "1a4131160493624086b65aa69c4e16ee69f3a6d251f3e521ab132b128b0b40f0466d915f0fcd7d9f4cc4bf0df3d1d725b72e12b60b5bec939556782e8e345039" },
                { "ia", "337f8eeca888b911aa1bafb8d1c981a69eff34808deb34db5ad1bfd2fd149bd90793864a181dae26880b641a4d06713a09a99bcb72d8f38823284471eb612a19" },
                { "id", "0fd0b13f9e8fa8d719c41b56a7082f6f49509d37df5402de2fe0436ea895e912b324f8ca2f7cd374d37edff01d0a0b225a89f5fbd7c4d7035123f77f5756cd1f" },
                { "is", "a1173037d5c6c487de1896e7b0d4924c2d1451d165a8f825e44640b1ecfb540f8eb7e8f1ebe25d11b2a76c151058773fabf06b408821a2bd40fa63409fc4e0cd" },
                { "it", "b72c1e1e5d2bbdd2b4602acbef46007f61c355ec2b3ff22d9220cfcd06580091bb7ec6a9aadc813124196865c2f101996810fc4a512639ea9b65f07d0ec664da" },
                { "ja", "be278a159da229ef01c1eef5ee95bfa1840382b27796050cca156bfc68fca6345e7bc1f670260d964aa322558d41cc45a3b2f8b8ed616a77501bb736aceae8d1" },
                { "ka", "df954196293c5654946df594038f0bb0773c3ebf1730986295720eb8a52eced3d2b8885931f836db5e2f82efeba842659170b9379c890523f992dc3ba0bebedd" },
                { "kab", "8c7901751df7ea8c9649d3741f991531a6af1d6e830a64b186b26316be18ae946155557ce38a62177c6f3e96820036fcb9e26c500678358743939f9f27d017af" },
                { "kk", "90e3da068863c9b36418e8c7b47c4bda39701949bd173bef217b115ffcd5d5ef52d6461e52da81e95cde4119dc30774ed28853f14990d8c32a747079ef5f367d" },
                { "km", "20d336f419117a358eebc23ea49aef460720667551a90d7b8955e1f70530ab7fc111722164d5a115e52a0a42f5efd58a7cc56609776f349f4fed33da30722670" },
                { "kn", "09973239b3b9e5f6cac39f5a9cbb537ac4f778dee3bfa3e131f0eab30bc732b88d66d38add74adbe714db05f9c1b1da09722a03d9031bc06a8edc958efacd40d" },
                { "ko", "80e280fb7fac3054dd26cac91a0b21943444379134daa17b45d0422d4ec29e45fb69d4a8a905e6d6f3271f37193b982794c8e4c4ac39e3f4ef5bf04754de0fbc" },
                { "lij", "9019b3f785657c63b89033bd93a610fcc858c05883da9e3e99d5caf8492d7aea8acb9f9109647432894becdfcb5bb03ef5f46a44ee16bd39c31b1bc3d877d2a5" },
                { "lt", "524951cf10c0e15aa4eca29f499f384a3523c8656b3bae82c55b8fc5065df2dcc34e4fbd6932d0c2cbbf61eb84f43b79dae54eebae6fbeae8eef15451c4beed0" },
                { "lv", "c7383a0d5bab7b2bb60216e143bc428082b09438f0a705a90cbdb8f2b88bc13179ff33ac40b0c4fe4798f36df94830df750af3f3d8f772e1a6fb7121ef4f8748" },
                { "mk", "1f450c6f4ce031cd34daa9bedaf431ef718bfd4f4cd704c68ec8464578da82a84daa7b1790b58569eceac1333fb4d26c2bac68a85ab451df18fce60df124d3ca" },
                { "mr", "546060dca230becade821e08bb24e8deda7aa7deeed481a3d97ee72798a6a0bbed66faa7034f7b69bbdbd2e6b5bc709ee6dca408ae05d6142eec94c40030c998" },
                { "ms", "6cc21862cb2fc6cfb704ba5939824d7f5ea115c1d4394ae7351201996e72aeed308d5ae801c7028654b95f31ddb4fc9318aacdaa66066fedbfe91c9da40ca546" },
                { "my", "69070e22e605c5d63d775eca2c6ab9d83e9d595bd8949018f862ab8c13ead9750f49bfb1e306109c6dbe0ade1a407230fbfe6ecc2328dcab538d221db3dec8cc" },
                { "nb-NO", "01467165243af0cd74da3f4961eb4d2364ac1a429b98d1f6564659322feb168659df41e15304a97df6145a5d852823f44b17c872a0b63d1669f19e289082f292" },
                { "ne-NP", "6fb20e3ea1a1493f63cefc0ebc265a6e1d2af2f1b21d819522af7734b80db1c8e90461bb97fc027f63af7aa1d9a9c52dc3a97165c6ffa48c1990f2491b8430f8" },
                { "nl", "bf260413a1b29f26b99fe8d23c9da146c1efe9f627e68e21786dcd5710dbafcd46a78496acc764854e76bc31de7bdfabeded6e16681d8e7848cba7b2d9b310bd" },
                { "nn-NO", "e6b7b0d2f116998b78059dbfd1456d85a2e2e1a1707bcc40f3db2893344d3f03c9baf3e0656375d221a98b8361ebc2f25f30d3f51f28978f57bbbe4995e8baa1" },
                { "oc", "5210f96392475e2700cb2f3b2a5b5431ad43a0e5ddb0a02c8f3fe82de6df13c605d027d96a9f67cad197e61cbdb54bd506474332b5ee18edde8104c09ba4f18e" },
                { "pa-IN", "410ed9513dddddbaf90195fca23b06a134140f43bca0d3f39530757f9bc8dbb4662bb0b7c52693bd6386f662eaa902eb24a32fcd1fc21dacfd03a7be363d22d2" },
                { "pl", "d9c53a85e3c08bb021fef97fa7db5f227f50d34a7b60d173b2d7c9c6cca749c7885363b045fbd9729a2765d20ba4a94a4ae7751c111ea32fac167b1b531fdb40" },
                { "pt-BR", "1e6bba5fbef1378d700a81f8b9232b1485c5f7b1caf54a30fc43a2a219aa3681d28ca95784403138511681504aad84603463de9053dd22781dc978c8beb39469" },
                { "pt-PT", "c2de5a6dadfcdd89ff1db8b19aba0eb8ce5067e3d7ca315007c67fc157ca40995bda0c277803c6a665dd79246efab8333ca995179683354434ef1d08a92c7552" },
                { "rm", "b001b729b95e5179248fd1a6fb379621139e1d817473c94ce8e850e5ec17c324f746dd3a9b11bcadb7c8e234ff8ac2da1aa9e0d419beee2c470a7c073e2dc54e" },
                { "ro", "dd9f1efe3d4fe5a5c6667c325f829dc304e49cc462a05ee959484af14050ea5b4136078b9ac6253889cf28aedee361ef18220b787489bb4ec8ab4eeb745f8117" },
                { "ru", "de042879be7ff65a8ea729e58bf6404d5da5157fe726515535d8950211b9bb11671538f0d4bf71bc10682edffac4f3993ca911a60e55dde275a5f605214c8217" },
                { "sat", "960af6ae3a41778d918fe0e2358736ca6125f57f19c93b1fb84a1cb1c12a6367170cc8591e360e3c0b5af35458fb2d2325ef7041f3c58855b3fd307c91b8f728" },
                { "sc", "207ca1c71c3ff15d31b7141e8ebc59a7a82573678dbd609a42902769a53ac2eb2d4255932c15f7bea6443cdb438c873f3d7bf7fc26d38b60328b0fed7b9b0eef" },
                { "sco", "69413a76d408d9ec2a19c255c8247ec8429bcbaca8482379cbc3effafbd7d1dbb1c5fae5555201ffa1583402274a8fe8c5e17967c33c2eb7cfcebc41b53bf741" },
                { "si", "cf68323910912d1d56e246e31929b78df92abb4c4ce6dfcc9343fdad61b810c5dfbc2d384a58ad90e21b653a2bc56894f8c1e957fe1ea979a8eb1220105940bb" },
                { "sk", "c045c579cf066fcd8bfa9af2fec03a5095272c4af21b4d048b742b395062d748c2b99271fb1194ba467c732b7e774b7e23a821de5c22303bf8bbdb524978bab9" },
                { "sl", "877185d4294c0a8a155bfdb363fd9b5511681485f60b9de6de341ba5cde5edd72e9ba250a8f94e52b0b0937a056ac09d9cc21af062d1e0c77c2298db5594c64d" },
                { "son", "149e7fc77daf93f60a8841242fe0cb41e9c9d235425b30bb0bad4446971640c511bc68ddfdb688c1906278ab04eb9cd487383cd218a384c53e628c40ab24d520" },
                { "sq", "785090c21fc7996864e47694d8e2c97c57746e6648cacfad2d61c17e5b4e09aacc247b54941ef7281c2b974b0d83ddd67d8a869584d5548c8b042474699febb0" },
                { "sr", "0631dcddcb01bae918de3fbd87c07eb9e4f4d75954af5a8c167857f35837ee62f3c57f94605859a45110a0498aa80c374d370112cef61646b299cb1f13bf0211" },
                { "sv-SE", "9acffb2850a9c3c4cf95e4c86191fe3bd4dba24559c763ad091b276a136b645958bc60a3398daedfe64c6773f7cfc7e9b7f3572db4aee65deb5822ed396302e2" },
                { "szl", "15a2cdee28ef930acbc3ca03273944a429181a1d9c28442c0b927524eedefff42803e1de1d94a5b831c13a26eeed366158bbad618d2d1448cd81d8fa5d209a0b" },
                { "ta", "90ef6be8bfe5992755fbb408bad6a4cb489205cbc7272f2ee4be4db103a9767164742ba443bfe2a8ddd7b8840200357a90f3c40c7438ae4aee7a72c1b92bdd03" },
                { "te", "2613e931b696da56e940f5a992e5525c1b7efc85212649d203c96de4bce3f5addcc8b535270cce999230be3c70c06bf0253704c82860a6e68dcbcf409db4ac07" },
                { "tg", "3ccb2c9dd40a0e8ecf6b5862ac2e28cfeae99e21607d99d31fdc134494195c8c1f0e4ca7560a4199472c4ec08120cf9083f66d0af3b068c2b43c1d0a40cb0148" },
                { "th", "c848bee2e661ad00f30dd20ba751f510cddc964a75abaa8a93891154b4fbfede553acb94f898b87c1d300d981d499ea0dc22f00caf764f5b19db28197f0ee4b2" },
                { "tl", "af7bccc69245ae2cf70c3a041cce87c56ccecf6c1f3c05b0ad37ea04d65ad2c2a7dd3dc312a51cb92a22b6cb07f8fb4803e8a3fccbea819b8bca6979cc75a1db" },
                { "tr", "6e6cd02050d2491ca0b906a3a7edc53f2f6480f5fc3d73640a27ecd38026adab0c70ba731ee74a5300c95122aab12eea89aa9f8cf7131e6d1aadebd29409baff" },
                { "trs", "23dd1c434aa317ff24d7c9f7e0cd791f639820be5e69d642ee6509404886196072b1c37b6dfb9d2f133c2e3e915a44d756a6345a3111091fbd737fe5e6284b19" },
                { "uk", "e364f11e5d2f5adc97e4e7fee632cefc03d71f4d6185c77dbb6497e657e632ede8daa18605d757799cfb20be9c2fefe5ecb9b78172a4d8921847826b5fe7ceb1" },
                { "ur", "f9e577611aa3eb829db623083d695906e5c26fcd225c4a40a20b9c07a7e435a3c1c25ea71b2cdfec6ca0bb8eb714ca955d754217dce88265ffc8acd422640cb3" },
                { "uz", "110d88016b56db7e62482d99d0d76f660aae599558c8c87bc85b7e3c8b818a6aad177cd2e8e14bce9d1bb3ec961badc098c734efef8c582e31dbd08a3117490a" },
                { "vi", "a1b471a9c68f2d238e3f830d4bd00fbeb902fdab89423601f5888973ec804fca793d2e22ab1dafaac3c00e6090b46caf260c86ea8968316cd91a86f4a3cbe7cb" },
                { "xh", "c413a113b76999aab3bce92e86262e5b8f58ab03e168c56929d656036f8dfbf078112aaebe338a01608813b8b760d244932ec0cec73d4d19987312430b0a37ed" },
                { "zh-CN", "8e529be1069c57c819d07075806e90b05a1b4cda1652ce44c936d41e338394494de3f3b2b0fe61fc2e4896651cd9ce3c783037bf92247bf5b095a024f1011d9a" },
                { "zh-TW", "662bed67d38c7277bd0ec90036e9b3f791a49b14b618237c37c4985b4a729face06c0254145edf952d98872deb442b14876849c20c52a202283f37bd465e15f0" }
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
