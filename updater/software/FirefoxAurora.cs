﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Net;
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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "94.0b7";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/94.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "2ba10b47809162565096f19d4c78ad717c4289754f079e4b29d24d9865ad0db7af760c2f0428a8e837f2388ca47a81714bd54dd19f784b63851bb1947c075e66" },
                { "af", "a868cb8980b4e76bada712837dd60e9853bc1386bd0ea1e13f8372724ac92225c28e8f713319ecc2095aec0a7ddfeca941a5f54b42bcbbd41f25bd0a647bec11" },
                { "an", "6674b3dd125ecb6cce5945d2b4ea5e1b20e3bbf1c2ae90b56d8c498c7de29867c6d152a0c866dedce73e0611083774389e87b8b818c6766777adcbce8bd53c45" },
                { "ar", "279cdeba3ef3fdb6338589b7d9a0ad5cd58d2312aa1b547d31cd331249b7576ee32ec9aaa4a3bf4c782420de2587dedd3bc0697946a60ae708b85abf58fdd220" },
                { "ast", "72686843cf6c3cf38b9fd1c1d75db87fda80ec53dce30b9c3fca303d6a5bc021f1ee700e565d1f981d2b6c2f20b9d914b4473a80ebf27b7da9c0c4b366936717" },
                { "az", "9710098a4800f1150173e91826b1bc18d6f4c4006ab9940ef3db908748e69e2ad37bb66c23910e20666262c4dc45ddcc272231ec4bffd239ced43937f4773651" },
                { "be", "e38c23d152806e4257e4da3b2aeb76b5f9e8250eae7846ae8c2aad53c64f6e0985af39b622da79d9d084c0e69f7ac5f2d23e7e2429d285a7e2116be5e2c84811" },
                { "bg", "12ff497eaf05d71326f08bca541b6418507e35f25647300eb9c4b58148b1a2cef0f013afb66ebd5e5141caaf6ccb49df2b0cd4aa24e9f226abdd7ee1502512bb" },
                { "bn", "f8bf4d8fe1f9b5f4c0801374abe25c31d1261066b9f6c3ab9ee93177d5a624b0e133676b74d1245e5137f6aab2dfe7d683b1cfc8c0f2aff6f0ea9ea6bc37d958" },
                { "br", "c309a5b9bb0d12048d7df1e28d2a7c6babb18f182ad655c94dc803770dcbdb16443a7e0c6d3d0410ee8b09af9f4198901ce69d0a8f46c7d3828bdcefd7c05179" },
                { "bs", "4143cb887065c83ce6e878d1e80f64659045d307a8eb7b876ce06d302ff7c2bf432e98c3ac0fd36486a7028fbc328e6931eb621bf22ba8671e366f7fa2dbe284" },
                { "ca", "8bbb83c2e989a58a9b7305829ef7f518b5a3244f063c6e50b7f685816f0df60e1cea03462033dea9dc53684fd36acbcd826380e8a80e927d2aa6d4f53a4d18a0" },
                { "cak", "fe438edaf0452d59f977c2d24fc57d775a4342ca70c7be0d21f1505079778afb4db4ad2f1d3ac2a0bb8a414777414935f0d2075575c0fb4ea71da2fcf8ca2270" },
                { "cs", "8b05902308dc746250acee29a9974f7248077cbae4dfb5d6bfd1536db37c1d74a205cfbb372badc89ee993dafccc31e575d46133711ce1eb460fa3f06363727a" },
                { "cy", "279f199c9cfe47bc341b744794b65da3474cc1baada7fd84347af48804bc93db766a8937fc0ad3f904c75c1082ed64a5da518e84382673b696cf7a3858521f5a" },
                { "da", "962853e887772bc55aee3478bd897d4598038c68480d06f234a4029d7964c31abd6402e53698c53b188c5f9776e769eb117c58b7d65a59e0b7f09509f40b5ca1" },
                { "de", "43cd7b6e1126e1ab6498a2c7a6aa2b712c119380129b6df597b2e72a8afcfa1ae776d28578c639922bcb549cd8e63ef9c3d3021ef36d2bde4e6bf3229f26a73e" },
                { "dsb", "25cdd8ca5b1a1cc9cf7cc5c39b7910377a78758c07ebf9427ab9cc475b805ec72ffcd5967446f8245228c0db4b1b25ea2ed1ab055812fec38d885ce87555b216" },
                { "el", "0cbb230d344df587e6adde6b936c92a53bf4eaaa7c03f989639a6e51278360ce70b9cc2f622fdbefbe1da62c64faee312e84a5f389905eeb429cb28c8179acc3" },
                { "en-CA", "336de2512d0f400e075eff8697aa2870964cd629669c1e03969af5b81b46565224d976d7d5194f7aadf8b30c0b06fe06af96ebf818c9edfa7504658dc7e0e5bd" },
                { "en-GB", "1fc5e3b26518d322547ab277eebe77a9254b9c0dfd06af64780c12101742d5d5cc6eb4b07d4785798d686e2a8b4348d3f0f504d74cc8581d8ab8fc8e8ba0ec06" },
                { "en-US", "52982393eba0bbd3ac8b547e7ff7aca66a8faae59475131279d96ab52ff9bc29ba8910d645ff2d9dba5a0143324a20cdce085b8db029d2025bea9d6288c8076c" },
                { "eo", "e44dc07525837be0b0f2750272bf1c66528f3d680a2567ba740bd7e5ce78985ef3381d65ae2be9ced874d00e66703a18a54f1e160efcb0849f9616875a0d4a88" },
                { "es-AR", "2e769ff1ec6e05a2ae8e340b8bab9d4193315b84165a8c6d5e69f777475ae62d6fc4cbb1617869b7c6e254bacbccab9a4c5f11f1c37032a506d922b6d2e0bebd" },
                { "es-CL", "239ee48d18cb4e4c4072e6c7a6954c6de9d1c861442165a2e8d397ebbc8e9700ad9e0631b62a729cb18cba1f004919fb393ee9e16a96d571032ff1c2c81b6011" },
                { "es-ES", "b3523727cc80e4019df5ad741c51f7fe26fb043e1737a1a43f9cc76bc2c65cfd3ef13d37a2e4ac2e2b214c6e58dbdce011679b9b1f61cc04ac3a9f896a5b09b8" },
                { "es-MX", "8186df57fdff5c71838b04d4137f11ee5e689ee6e41583abe5e61112046f96a15d8d4d65a34fc4a69f19c3bec945f8c232d19539bfd2c6a25cff8d9a18057fe8" },
                { "et", "1ad16fca27a964f43d628b5bf762201786deab700548d23afd1432c05916beba6e60ca17dec98f73b5971d435f98fcecb51b5a1dcc9b89f01034188f2eb5eba4" },
                { "eu", "27fe547fa14818817757d6aece83a2ddd9dd3dd9109a1fbbbad18d9409d10494d5b7234056b34f6d68287f7ffdebcd33d74ee91f99dc4f7e5c56d5ddcf621b23" },
                { "fa", "1de1bc7403724e5dbb2138df5f3f7e9639bacc86e33fa62008148675203077486b8506928b750bfeb412009529f11dc305b8590491a7aed574f0a0215553b3c3" },
                { "ff", "72b06e933c210bdeac7ec85f2018c62b3fcb1c28b1cb6871f2bb9a80be40833587aa70c9fb5a64625f78eaf124d7401d1d6d48187256dca0dea9bb0bc1c1f875" },
                { "fi", "58cd47c771a350ebf7334673b6d5decdc9b9b8cfd308827b0fd75b78a7c8a4f331c9240d20f14501f0cafb635cb353248943f8270eb1b6e3f1de2089c4077dbf" },
                { "fr", "5411b0eb922c8199ebca164520620ced89d1024d5286f25e48b0b8c5b444df3fe164e2427c6e125841fd4ce43d7d1f0560daa52a6a9fc4ad499ed92850889452" },
                { "fy-NL", "fc4396b0f917737a07593bb6d4357e687de654cfd03eac6a13324fa0b4524e01f6bc55590376a73a14dd48e6538f21b0807809e13c0612b026235968857c2672" },
                { "ga-IE", "4c0940d8df83eeefdfcd6fddea1d3e7170133a39f6e24ac6b5c88213326dd3cf0322f2cda3c1de4fc21f5212df2c68bf1979b49881526533154a3e8d19ac3ddd" },
                { "gd", "0296ae55609c44c6228b98ac44ea116e82f117b75cf04b2c04d9f13a14034e5e248a9ab5a9f96d223c653680e87aa60f14642ee53f10c2df1d6342d16212f759" },
                { "gl", "c072241a304678c04df9f71dc45155c64a9eb7b194f56b497732e2c70ccce31d6994494505023282e310e7a1648b9f7fb2f3dab512d19eb5a200b6fe2cb08efa" },
                { "gn", "ff9dc0160a0f800f281b0fe97a1ca65c2f93e9383e3f3e2630e665042d87ae79cba6ac092be5894dd53c395986b6f9ea515064a219fb2e7a8b2ab76a82c3e24d" },
                { "gu-IN", "c2db04ba1530d0b76d41f0df15ad87117494447f78d4dec89726048d65a2b9d5d31a7fb641b6f5f204fc031786d56764af342daa0778816d3c5948cf6eefd8a5" },
                { "he", "7fbfa50da045be9ad93121438c328531646e51362e2757e841ea719e900fbac2753ff178a9917c492175205cc3171b0bfd73ab43529526d6f0009e42158a7043" },
                { "hi-IN", "c38541a38d7725aee726dc0c27fd1aca92817188772929568febbf5dd4b58e3fbdc01ea6fc10275d469109f11577e9936643a96fec63d6ea2be60510fbe8b5e5" },
                { "hr", "d1c9ce1ed17a8db0667e64677b388d9cca207b5e2e8fff78ff02932682452ee6f6861f191612c6eb8b6bb56edce29ce0ca532ea7e9ab3562d2ce6c487ec11ac6" },
                { "hsb", "9bf9a00be35d182574d9b7f12b11c4cc436dd52d454f74ba27bfe557b58878325997fba2172f5a9fd82a8bdbbf99fd320f5eef42ba820898095be87c2e3d6c36" },
                { "hu", "a1fc2a21b2c3c75675e9e5d8ba7976342de6d78fd8458ca1ad7eb205cd7c1ab284f79297229e81245d66e78432c9cea975c86a0c24423fc0a41214e1341a1135" },
                { "hy-AM", "b899b6e610c8ebc699dac354913ed69ab71becd8c8f90453753b8bf9c8a88cb1c9fa704eeb9959635dec73064ecda7e9be9a960606ad508ff261530916120b33" },
                { "ia", "833d244883e5f48a1baa33fa84b8cbbec90a4f73c6fc9cd5f525879d888cebc868b8ccea2cd441dc70e392878e7e45452cce26fabb5e1f6efe1c7ab17a38426c" },
                { "id", "0a8c86c29587e1d435764cfdea90470078b678959c1b409866b78dd9ab30e3cdffdb2d1ed9d9c58171423224288926202ef10c09cc0e0c52e590d508ed6eea97" },
                { "is", "b2d03233dc1b9bbf34059a651c1af56074cca8af1d02f9e7bf14e20284b9b73b7ebe0062a85fdaac763af3a1c1a7a65a0e94d1409cc40908f449b3d64e2cef51" },
                { "it", "ef5123d72b64861b3b13f1bec4158d51d894802ca4c133ce132e6afc6117ba99fb96d9d5051a61c2effa70b47345986b8e270b6607dfa349abe6433499ca6d9d" },
                { "ja", "d46389843952001731a460af6295c1c57c32b7fc8c8ca0d125881db83bdbcadd1537301b09d96bb2ac49faef98368fd48348944e8d4442afbb98a65abda5d956" },
                { "ka", "4755a6dd836e491c5df8ba47def2b01c393615b3e7509c8a88aed463941da745cc2f41332cf84f068825ad0280368b7692ff5dc8164162129a51cd89d8352cfb" },
                { "kab", "045a054aba67e28b136c540e1d83e93f382a4ebfc4788f065787e7ea9265d641b91d7acccfc645c8f8ca053f68b87d76475992756f532460f2b3913ca0ade0aa" },
                { "kk", "5bc5ac5a8fe78fd377f8570d6f76bbb1f31f7b95423b82f523cd1b8bf0fa93e6ea5926b77597b03748a4005c7763762f2d08549da021a300ff9d4d80b5f69bf7" },
                { "km", "927b6867188e165c286a28bb21ef0ddb7399a44d3d625354b55a08b59e4136ba0c2098b11bf51c9faf998aedfecf36d17a7c2c28b312407dee1e5a83f633148e" },
                { "kn", "854fa46fbf067a4d3f5699834d4def9b442f0a713370c72e9a6fc5e8a678b20621ff5e318282e32b7f9829488c5607d753ea15b9985450424b53f97b33684943" },
                { "ko", "848849fc4821f36dd59e20fa908492e778cf303e400785765c2e2345e8d0dde9580cc9811c4ac0d52fe965eecc44c645fb01bc1abf3a42226561eca403d8af9a" },
                { "lij", "f42aa38da0991565a462993795266fbc305161661d7f8f940e7cafa5d8a71928852473b70d5746907316830e4da426aefed980c227432cd7bb9a3fb900f8f732" },
                { "lt", "108a1c19e388d8b0a1daa6778f4765620755006358de58723d3f4d99b024f88990d49af0ac60fc6389b77cabe81bc5bf686dbac697141fb12b226878fb9cbd5e" },
                { "lv", "94e372dc4534ad2d6fa486cc596157d0fdf34b679d9ebd7b7b6c3a64a4092c9539eeda5890b0f4d555351bf26c4629ee31278e6ca9eb3eaef0acb55250139431" },
                { "mk", "acb3ba80733dc436817971bf010df91773291c87e304305c9046a6851977ed032008ca1a83d151736d84d2a6b01e92ac0862196648665edaae57f7196169276d" },
                { "mr", "8bf41eb1cb95092609f4ca14f813c0710efbfb9790c88c7e2e77a60cd4f772fa501a76a1d3aeaa46c1d84fa53628d0e80ae130dbf61acb868dd10f3beadb333d" },
                { "ms", "f726931f66c8ae11d701e2d609b8e3205487e003b6683118efb8387bb0d378809ec9bfbbf90cec9b49e2d05900b190937fc31a3c1cdde4fe77258d6cfcdb921a" },
                { "my", "5d86bcda8f736c5735313a649525c6b62356102961e0d54d0c65d8f759b007e2e2196daa3f6232677ed24a720971c7abc37b5620e67bd2be90d063466704264b" },
                { "nb-NO", "8e5f4bcb480fe06ff92aa763481e9a56605c88a9a3747b298b49f84e948dacecbdc4b797d818bad769c848676c091ab7d65a9200466681025d2fba712a59b25e" },
                { "ne-NP", "37e216fa27be09cd7834277b88884d62a6a0bf800c4a8a249dd3e62f324b2d5a21257021917f8e9b0864b8513e48206e0d0989b7bdeece20be22eab42a48a052" },
                { "nl", "cae45c4a1a584480764e57a41f90a661bc704f0fd65275a9a6c526be2e5d49148792734dd9cc2acc83d43da0fb53c9d5e6e6706f051c728bfcd1c7cfd4fb091d" },
                { "nn-NO", "4bcd2b73d035c8778dc1f7312010958600de52fd8f76b8fc1e8bc657e35a1cac48db8ec49701d6b0af2ad94063e9aaab6d409ca33d3eeea5f1b09cc7061cf86b" },
                { "oc", "e324855f0df79beb854956b8615a8b5f3a564dc0037dbd62c01688b21d780c535e309250dd0a366a4e05df6d1e22e5ffe59d8c519ad14422e31d7da76005d422" },
                { "pa-IN", "92b9f3d46927c52dd27649b004d9e82439b8151366fb24bbeb9f71a9d4882b3a678622dc0715318c179d38370bbc545df3da4aff8328b30186b53796553022ba" },
                { "pl", "9936f5a6072e298daf064af40264199cfaae83753fdf896322fd43396940b5aee4121b5cf8b0ce08e55cfeb82c277c70bef1548717a41bc186becdb4f0b5ab2f" },
                { "pt-BR", "dd6038035649061567f1ef630e2efbc07458fa5797a5f522c6aee97f153364b817b9348c5ffcd2a6fd08f2f2f9f9f117289ec6147dd4c926c8e289dc349a0c3c" },
                { "pt-PT", "53bc419bcffd7519f38dfd994016afe1d8088382242febf76e971ebf60be0d51c789c11b7ecb2683c33ce7d7105c7a19a3477d878b9bdf36fc757fd58869fdfa" },
                { "rm", "a38f83d526f4792f6fd971739fd31b981b52e62627738fdad621f49661716180e614b8e494585641b8ccd077758e42fbbd3db77a43d66189858feb016f060f7a" },
                { "ro", "f435900fd1eddfaf50ab29d4446ca31f3ecd597d9bd0e83db9cae58353a189f5179a8be8fcb1f84059f040466cafd955f4f1fb326ef8346973f04da169b7c6d3" },
                { "ru", "63bf206fc57a54c5229b916d984781aedb95b1d3f77c79acc91424535012e1e30db6630cb45f86c39e54fc633ef20e175db14f13388952132c4ea5436c391269" },
                { "sco", "df97f7e3f8574d9640d98b38da75e6db5d7711e115d9c67a89f570899d04cee1467c591a15865957d33ee90c7cb3d9c1823062141ed5aa63a875d6e475939e66" },
                { "si", "a2a6b85c47f03346e3c6fe2fb9fcad48f63df5f4f936e87a67c7ceb765c39018e2931472296921c8d844c0d72e424c6a052f6602f3bff6ffea6c9b916600080e" },
                { "sk", "3d6784fe9367a22dd14ae7fc06d7cb73baab954a69686df0b1f7b2a18b8367f450312fb69f36985e6d508e42e25af72c47ab0559cd53fade392fa4068bb5f61f" },
                { "sl", "fa8f999290f6b1ce1afed328991165a029e4b41680890deeab6303ba0f5524f4f821fba38c14b962022fb9d01e0fa05aa91c42fc3f4c174ddd091ed50137958d" },
                { "son", "218211784f15a686fded6091d06a4a6a17d6533bef758c38a74036c4f4b1b772ceb1e682e71e5520c54186e5fb8d5b1a670e9c596b61d52d114fd3c36bcafea8" },
                { "sq", "2ce5726272b5e49c92dbf57cdb75f23db46ca7793a22b22fc71ec73f702c58054d7341448aca857c1de97799f2fc34f1c7e4f1b383e5839b166d08c278c3a2c6" },
                { "sr", "ec3b614ce21232a6bf457603f2cbca558460299e50ec2850370ebfef9b659c52d883f6e0f117af862d68709c3dea389d4df2698e8d7347e7a43f8e9da9d5b594" },
                { "sv-SE", "b24fbc323089f588b4b4bfd41c4722d9bfa847a47f884ffbb12456d7b51657b74f108508e531691b6e1628b1576b8f0ea86bb85ae23db7d3468329a46a7501bd" },
                { "szl", "2498bfcdbff618be18f0467d8397e08aadfefbd2cfacc816486df717af57acccabc16671a84c0ff695072f325c9feb9618bf6b20539c897b4e7742a6bc3414e3" },
                { "ta", "57cc002e75fa39dfffb3e3c708823a293fcee160f959b7f78b94735ff735384e1391b7208dc4fd4b16abc37ad7dec105d5fc24ba16411b135dcfe307831dc825" },
                { "te", "2708b59b5b598836421ffa949df0abb345c57651245513c71bdc3c342849f8e90b33396233f9c06a3abd267a3d22ae67096174c97f85489399a3db2468f5395c" },
                { "th", "82d02955ca2a0ac6dde2829bc52f093f7d994d66b9faa4f4e42022e885e4d8723addb77dac88975c3fb7364d47afdbc6c531ab6d42cccd1379f1e33530fe5a9b" },
                { "tl", "442a0fbef9bfd8c58f66c970c5475776d08bbf4acd7dc0dda1e0b447c6c6862f19d9f6fcabbf045693e2533f61ab7f9ee5d2535a916be6a1b013b2b2671a0089" },
                { "tr", "584cb13a9e0f1c70c47338e3752415c1e7f9a0a8cfdd8580fc89936b3c35f72b75f58077f947cd5349f888136ce783b58355a9c8317ef313cdca36d4a87d6085" },
                { "trs", "c2dc6c5d9ea307ddf110bb530f5246a3619250a71173ea22ffb65237a9dbf216b0af146e516c9789bb87072a2a0e18099ea6ed281302f3819cb73b0cc9f3f881" },
                { "uk", "6ca6a7dfa9c477e862f097ce3178a918bc397435b15abbb8ad94e5d5995b3b8226325635757bf596ace90f6b870ba9cebe26843ce64b6a200efbd2ec880e4042" },
                { "ur", "c22b76b3a98831818e646cad74ce8d48f496581c832a874b1a24147e98c7e934ff55a3d17282420de9d095daf608e6fa258b598ec0f7191584adfda3abbc6056" },
                { "uz", "c241f4efbc6398220a60ddad84064b3e2266ca18691ecc4964aa6a991c4fbb359f11740696c64363072f7eeda35a35034236f165825b072565e5c7e18bc847af" },
                { "vi", "63037c7327cf59cbb960ff7ed079c9b8a24426973196d6bcc973fa70f1075b1e07bd01521b26f5b18efa25753996cd43463a8a0988f5abd2a8bdfa88f961dd9f" },
                { "xh", "92fd6f02b3e3b0c6b574d54833d8f5c49bef5b66ff34a87eb96842c473de3b6ddc9f526b9d30ca16e231730bfe7187926bba92d8515035c03f4a4572040d4854" },
                { "zh-CN", "17db7e08e1b6ec28c3e42d25bdcba08d1e59280e1e9bc86cb7b3bde74d26acc1ff149c6e263cb72699f56fae24f065767d6f18cf333496b72467d443e4e9f6cc" },
                { "zh-TW", "664404f278d91034e159ca4ac61be118d37011809511b684e213858ed3a5e1655787d5c31cc46a6addc83b8232f98ce74f1d2f60699228223b7697f747a539b0" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/94.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "45110cbb54bbdce732df5a52c1b4ca2d529a6eae6faaa374cdc0099d723875c54a572fcc483c4bab2b27104b6c6a6e19e91090876e389bc424b24691893d790c" },
                { "af", "86e156a3bf0879c7609bf395740c719774939608b0c3c9cd9259c5d489f07d15ef325cb69897b7e45a6026fd3090b63501a3b6116cc18d16335555b591c9c0df" },
                { "an", "7eefe9ccd876f4361b8ae172e16c04cbe88b9d8c417bdd074688946a2056608f0c4f7ae95ff7fdaeef3efe50067453748bae90e5ad7f3061a38a2c62839eded6" },
                { "ar", "07589b7600b97fd34dcb977376ec5685379f4c886d44b5d44c6dea337217ac2dca55a43c1bed2d6ff1d73ba0b4881dfdd3dac3f0d93cc4ad5d5cdde4babc9b35" },
                { "ast", "a79b97f7a6f71880edbbccd274d77700478247d47e15945baa6c4986961e5b942fe0e5d4f526610c97a7a3f5bdd83452cabe3ae090b2455a6a3bf536e3c77923" },
                { "az", "f79a27819a62bf10ed043507ca6586ed2c544e6fa51c31567eb949e5aadaef0ccd39a7d8bc4174d664632efced5cb6b2d3c1e789d9ae5d021df52907a0470e65" },
                { "be", "13356a80b40063c5a125b9e63de7b5b55d69f5e538d8521d814bc601362dab6c7b3cc772357a9bfe5af1d075defe691e129dbf59ef96e16e7c0117e39b08bad3" },
                { "bg", "880eea37d5f69208cf88106351474599b535a0394860de33bf0df0715e4a83855227fe203e99c1853eba8f70ca20399d1575997525921a4703eeea5619d3a369" },
                { "bn", "28b80aadd4f3e10f8824a2300f8c90e91f0935f84efb33f3b50077ed788556145b205148f15124f5900c2970461596ac78874ffd97042c267325c299a27d2188" },
                { "br", "879ba01fa0cd68673649e7d070a3916b38366a1c7c0052a7863cbe61db425794ce5491d5a294a2765d0125ca9e8d3009633cd3eea52cd02476ceab3f54d0aadf" },
                { "bs", "15ec9308c47a0720a22f4ee55fa9b72f10e9eb74218ce934613b7114733422175574f6556e3fefae2f5d437e88fc3e5cfab21a6d9f4c4a4a005bac3a3d0f1275" },
                { "ca", "b6a6bf21908d68640848aac964038bc59e51b349050f3d1b5a2b421df538a953b42f9683ee91f706f2037c4588b225172eca4f4b864c7be249137a4c7ec4bff8" },
                { "cak", "7419a033c7ac9aac7fa898e5286346ac6d2423e512c5d438c82b7696250e788e1fbf845b5c605c2cd20bd8db9cd1f6aa3d12daf6a8a9be121df76542221ae10c" },
                { "cs", "4a88925a71bb7a56f93592fef77790033b427aabe68ad7c6e6fe99a5827ecacec38521a30b7ffd171e334fbe91763b543a41a4cc8fa0ffcd963d098e51614d0e" },
                { "cy", "7ad3ccac610f061bf281ac7db242c04c8726aeb3f1321ed1f61fddf12c5f53ede873b9d0c27b4db09e06d54836e83b85232f9dedd095ba4baf6e81cad43311a9" },
                { "da", "033c8568c652762c1b6545e22a7b2663b2676641abfde374d8745e3b1c468ca93f4a6c0077dc86333e1f391242b17592f0c4bf63989d02833badff08bb04532f" },
                { "de", "1505fa80aa10373a66da4878e09a6aacf444ef181d74ce84a7aa256e9cc3d647a5c065f2732bf7c15ba00277899e3608360df67aff8f47f2606ec9b06f098c1a" },
                { "dsb", "350f33d2ede564898bfbbf9581157c264d5da0b2dc4ea9d4adca31f7522e3cdffd327b27e5a0970a8193be6bad16f923ce5f8fe2f19f639a63ee7b4376d337b7" },
                { "el", "baaf0ef2932432543fc69c80b49ae7269bc249a868058921ad936bf9620c40ec210b15cbf3ed8fab3ffa9d7a9b6a6777b3fbe59ff21b3f635b8ec5d95a1bfaf1" },
                { "en-CA", "ccdf454f04ce9e4a28d4d0dff2ef0dca929c5420e0bd3e3295f296b55f946577fa59f71114bb511cb41abda2c9fe67271ee754ad8cf69be829107c0a09c42f3f" },
                { "en-GB", "1f31c201001418397a2317c498feac031e6e6e2b0466448ae67311fc5b90d4251508a3493d1011fb452e1f4860a707d84230391aa259744e14133f061b1c2c4d" },
                { "en-US", "1545d531b4256dd2bd867239805907fc4393af4254399d548f775c49a971d1b48fef4f71350d8c03cd3a9586edc456b890a78a92e211e03ca7458915b29d6a30" },
                { "eo", "a050aa90dcfe33e411cf97a3a28a2bdfc985d963b8b0ca24e239768e0dc5cbd6c61eabf2e7e404c9232ac658ef0e4195631a2d1fd0df5a4be5b550a144b37561" },
                { "es-AR", "0ff35ed963ce432addf35adafccc09c034f3203c49ef33027ebed9055fce37920643e1260c18b5c9ca70b901a1c295423c6db05cca538d6474f504b55bcaa7be" },
                { "es-CL", "6f0dffd5c60678b1b34101bc32692683b9bcc10f882264588db79c3643e172a062fd459ce543612531082b6ed8de0770d7a375ec4fa555f1915cb4135063d283" },
                { "es-ES", "9fa287d63365835d2c260b6c7f2d1b11be2544731442c8578794e46acd6ef5c3fbcb464903d2e239cc830b523ab42e8a8f4d2471c7f2f1e47688849353064f1a" },
                { "es-MX", "4c0d7956b6bac025b5f0195306d6b80a5ae124dee886838d68bb48a7847091508214feee8a5abe67b6f3f3fbcce977dedbb84b15478289f1b2f5a2df622b82cb" },
                { "et", "1f5f03770854dd3d04b8119cb5eaafbfea09b4808aad2391c8777644d3ffe6bfe100c42ba9bb6d2c6b88239daf88d75d7ad04c08711da1f62c9a72ac282efa6e" },
                { "eu", "9a2c6a90bc0b4d71d95b7e4aabc5c02f025d9becc1f4b5cde759e923fb5b4f40f59b8da2d5f856c116a717a2fbcb9679a3981f5c6866197a1afe301686146424" },
                { "fa", "7db717c366d1e97d5d2cc8a2e6b35bab12dd55bee887782639e3d684d59dcee996874104d4bc73c2e73a9aff6de6bc7251ef6bfeb9e29aeeb175ca07cb82888a" },
                { "ff", "ef7640a5cf8e1e77303c0f72d61668dc7482ea9cb34f68d61aed97c208db01125d713c7bb769736542c9a919d0f9e5b10414867f681d3a2e8622275648a3981e" },
                { "fi", "d33939c401caad467ac30fa000bf78fe2a1cdd6b3b3c2830b054005e5b597e39bc350349a419bff7c06a2c9df44b437d8f8b3c51920a11c99fc9cb20a53f9169" },
                { "fr", "0a0a2d28eae2640f64485660efbefed8a99278fcc10232028e5b2b6e642d95523eed05bf0555bbcf24fcb0419f535232b72a3962197e9f7dbd3af10853086b3c" },
                { "fy-NL", "20c9b6634f55b0fcc6f900b2ff2170c8a202b7b4c74d6e820bd39b9dd3c35e9d94aaaf5c41c75b0d04d96bdc9000404c57f5cc3def92663f962b86c497b802b8" },
                { "ga-IE", "1f45c97fee621487bde912cfae351ccbf01b139645a07c6bea28e67cf8c18dbe6cec0a421d77c76d1aebc61b9e889c47b9a7ea067110e0936c7a7b1a9015d057" },
                { "gd", "acd298f09b6e5cfc130276082474b6037bf5582cde96973c123ad0ff5f8216aa656a62197275d073efe6b79aec52ffa309a42b1616fa36db48d3e871e384ac02" },
                { "gl", "6a4b2c33297170baf1fc4d41f292992e1109414af5a53bef6519c63b407aa2027cdee7974b97feb88ed78c420fcb33b65ef69a0a4de9863d559d7a099f09115e" },
                { "gn", "6631967e877a1c2c06365ea5fb524d49a635d01b47257b2977ea593f4d05d0a12fe37d526817617f62c28dbf1940aa54be106f15ca4cf6aa8aa8070d1c0df507" },
                { "gu-IN", "d258bec53c1b31a33875212d8ab12527643af14ecdf9826dce00f079a837499fb61efe375bbfcd5e8b32122ec2eaa55d9dff30bd9b98c228a90bc380ca76aab5" },
                { "he", "8964dd6a35de9ef1f033ae7815b2898c3184316c37cd4959685826eb0c389df4a46e08c1d3447bd3abd6f916fed53b8c64fe20eea02a8baf403165436972f791" },
                { "hi-IN", "0238af9876fbf52c0bdfe10b851bf41c4c5a93b8387eae9666f0156abdc723f8310aa5f3dbac553aab62e849fb3b93e7ffac67c2f4a6fbd376bb99a937b6a58a" },
                { "hr", "1c4be608682dd292a87f8ad77312fffaca6b9490f710c24dd74c40f9570fa168cc693a1dee759b374e4f4afb490ab6e2279e473aa95ed42ba8b0dab3a1b77485" },
                { "hsb", "49d17aba9f4a33c5e77b662f10bb6c172104f9a5c62f114bae5efeeef528ee6b42869833889c950320353e9c0acfdde7cd95f217205139c1acc1de2c9aa06152" },
                { "hu", "531b049164e124bbaf306f27bca986bdf139e13dc31a0cf1703b0dd6072b7dea4209338e243e2c4c348dceb7fb101a96af72aa02c28f6b7ab091f1d667cedbae" },
                { "hy-AM", "6be65a826580b7478545525323fc9659a67b8baf6aef02ab690739df0d599e541fffa58d1ed863b6956203638c4ddbc97ae8f33c2f32c5efb6ec253c8aff6adf" },
                { "ia", "d19eb6be54c463a37412bde9b926769f005671dd688e60addb9418c4ab45bdcb8acc56eace173824eaf216c700e693c8ca0a90e331b4f71d47b03bced095c037" },
                { "id", "5871d4d39282fd60496288006021021796a243fdf693791e022b25eb008aa946ce34a03a7fc178ecb2f8cf290ad163f0ee117b5aa955db2813097cc95d3c707c" },
                { "is", "6f0e45e07a245aa5ff4c0e3af3c919a44e23628763d6d8387bd1c794aa366ea2940c0744a2246e511514d6f4f17e07573340737a5edbb1b5e2fc5124a2f5fb33" },
                { "it", "bbba6d265640d43092e3047cd43aac27c9a56e00c9529e9eb245476b2cf01a2728b3df1ff4c04d1c146871869f876f2066d4635083d2d6671ff4ce43e704e290" },
                { "ja", "d018c49aeafbdaef8c2033866cb4a74d12421359084d571f0fd898a72e48e602fa16d198ee2f46f409ca7c823c2d84b33735fd8ce2af5a4a42514945ce147745" },
                { "ka", "f27997535d0418ee415aeeeb07cc330ecda4fd6946ef1199f01630d9b8ec371672eafb01c73f455c1dd9e7b6fea28944822e41aded1e62f270ad163c4ab4076e" },
                { "kab", "1549ef168803db14772188c033e87411f518d6d41157a68cf10da1a9cfc9380bfff8d10e2029374e6e5d6065a33eece0b0ab7e2bd58390eb3358b6ef86bac540" },
                { "kk", "204945e24f89e2a10038b7c8c9ba5902481c18f9fac29d40309396770128f384fb992b868f55bf5d048fd90019b94cd4b9b47fd6d2913985f45c683f88bb2145" },
                { "km", "594953078bf12b52ad65661908d9cd7e46e593b29b30a73d6c23ce26f4eb03b1c85d89381fae6a04da84b42f6c7ea222657b2059560224857ff2bc344a635b7f" },
                { "kn", "f251a04a6b8c6ae42a50c7774a6d073d6d0adaeca90bc0d4dd2d4a5c01dabbb0d4765c304c960600c21ac018e6383c2aa5dc5e20275cae5d4679a5a8d61f9dcd" },
                { "ko", "2981699db988952f4ad9cccb28a08cf42a616ae95c99d4827da861749d6ad17a3004fac56a7364e64677945a7d1ff3c105b6aafb4b1c9f6fabcd3388cd2217e8" },
                { "lij", "9eed94833de309d15b75e8f956c5acc6a9c3e7707289369a31837e8e85125f4b825b1640c9b9d794ea18e4d47eee6fb50577ac3977ba615bbd4f9d339ad86a28" },
                { "lt", "9d3f41a18d56b6e4bd7f0b64de1c42a14d049419b13c2cecd9460f6eec03f0561ae27ef38bca4cd75c59a8f6b62ccc408301b2aa433f9905de584b958783c677" },
                { "lv", "f2c7d14b2e10cea3b2f4ed9f7e35611dc136fae3991596a6198db9a650e0a9f30aa8ad3b8e47f173ceb077e36f3a7ddb97afb1051bdad86dacf1ba64ec79bc9e" },
                { "mk", "50a29fcb8f551045ebff8784be706617a00c8ca4334a3ff24ded2e929e9600697b14b7bea4fe8167f459b085f3f3c45681911e4a6bbd3f44c8e0db0e0be8a8e6" },
                { "mr", "0154273c4c50929a720d0db96e74eeae20e0e03a9e6d62e256d94bd189f2ec813c50b960a6b6e2d28540bb36b343d41f5984ce808796ceb784b5a0a433faeb98" },
                { "ms", "e34a52426bc655c0b8f115a58b6b6031c0afec759943e3a502a80de9e0c5dd5a21e31fc9f249db32ddbc60f17a1e27a5d4a3264862b06c3658d68d3b6e1c3847" },
                { "my", "409dc7737fd159935bdf39b280d4508685c1275d1ecea190e39b2edb2183bff3562abaab7ae52fa6d9437024b67d7289cdd82c40131bbdda58f75b2fba463f04" },
                { "nb-NO", "b7bcede907cf240ae65b444a9e88e6b0043eb246276b5191b3bd92caecb89e7f95df87f8ba649b0de89956f3ab724bf1564e864dee07e9aa97a5cef1d11f2d85" },
                { "ne-NP", "4e9495eb3d3fc409f6077549477ac3b1e4ac6ef08fd59d9a25785d4754a6ee8b8344108b4e23aeab930a79fb02e2343d4c1d377f4f03277bd7218ba46f83953f" },
                { "nl", "9013dd6247b1e20a54791cf792c79fb3ba60516d0ad3ec4aa8a829a85c71daaebbe3e92d747a86798b869de4263c33e58c4b894a2283997bb401921f78eda319" },
                { "nn-NO", "2576a21abc4e2ae7bb7e0cf53a3db8eb9a63870aec4a7a8ae488aa10a7acd288a5cdcef80590ec7a983383f85855f267908a1efa82b93138660af498f51c6cb8" },
                { "oc", "0d6898cb8d3f97c6913e13a70ba831b16680631e071fac9b7a77c6e5667da84feb8d6c1e371f911ea5cc39857dce0ead2b23c782df407d3d0bbd853516701149" },
                { "pa-IN", "ea76fe2a3e585ebde63c292875f2edb23c16fcbcf8f692bcc50f6fa765f6b020a8fba3e0d4994b38ea157a99e2cc450672416c01609b9f2659359b8b635ea713" },
                { "pl", "c8ca986548f07a259b33771608eb5245060170c88a9518910a7098d70e3b9fa6265d54215fbfb31f8dc5381fdac0a5e0a8b743c9b7479d9ae2e987575ef6ca09" },
                { "pt-BR", "7867b7df95ba24c3cc4b39ea6c94d4507ae740740dd6af7e70ab2515c83ae51673c38d51b0f17b11114d01a7abed64dd72963d687aa540bb563f4f3a764151d6" },
                { "pt-PT", "03e0fab2374c993f0aa0a156268e5b0e854c3a6d9d5be1a40b9a9c6203ffb6c520897f392d0aac7aa35540b408d3588d3e766a8ed07d1b7be046e802a6eb6664" },
                { "rm", "7622a1c1c3844cd59fc9b164767386c00fc3f960c7da2dd39b157920e20ac4f99189443fddae16e13496b1331d33d46d39edc2022407e7299f108631a9ff07f8" },
                { "ro", "c419e1e52ffab2fb06e990797d68a6d8fb9f49159a7b5f237b08e453a150bf6086754ea01552437c05e51d25759b00c334388903f2a6ab0708cd0a0789d516f3" },
                { "ru", "f234764f7ad59ef782ad39d03d36e5fc6c44ca9088e2034380c74028fecc0735950d02482e2de5c3a0c95cc594b86b66ad690e27fb8d733fd0bf9e7ed27c65ab" },
                { "sco", "9f881a3b3f9081771f304af7ddab26185781f8a8b1a1f78a2d8b11af84ae3032ba9e10dec4dfcf3558997be7bb8e78c880e80b9c1dd2b10aabf6f57c27bbe44e" },
                { "si", "0b0bdc0c59ce57353e092498f888e2c4edc1b857f23af0d46b87fab25dd1a82cfcb7ad65f62168ae538f7b87662a53d763d8470ab7a2b77033dab50986bde56c" },
                { "sk", "520d744e66dbb0d35e38a631c5d097fefbb82d957454612d321818021442bbb2198bf2e05d3e5c1bec3d6995e79ad214d9a000802cfba2fcd3d189273c09d772" },
                { "sl", "20c41b7b29e3db930ab9c3283f82a11cf856496dbe36af2040e0f0d36253ee29dde3fac62325cbb1a794b06d51e02e3a086a20ba2acfb82eb5a1e84e0f6658b2" },
                { "son", "005956efa2fb9e98caa5eac982949b8fe983f716c7f315feea3ba8fc53f573656c0b7960d40864e3cdfa5164a7867d81c8cc5732cdb6445dadf61d14cd2f53b1" },
                { "sq", "eb613427acf70aa2d562f0e41b491e73b5071e8ab2e9dd90902c74c6eca0c2ebd88b76a7ba5654aff8e5c54a96c621a265078926dd687bb12a464f99b4b86bff" },
                { "sr", "10eb6682782c07402d8058c75071e9962812e4ae58eab26e0805056ff73ea3daafb4df32335d0c53be21a9247ab1555c60ac7a82e666a76b0e5c1495deafe82d" },
                { "sv-SE", "1d597662730be528dc52dd0319bbf5c654005e44867eccfd3b5ac75ab2e04d1fb30a4c08fff258ab9bff8e31d5e86868417277571fd52c19123ee088f003c149" },
                { "szl", "20f823a8c7b0c027cdfd8b02bca17f6d5b180d4fe8e91cdff754f0e109c2546891a51fbb9d1b62bb094a6caa8a8d66f9e44bd5e8299f82d813a9c6936e103355" },
                { "ta", "2d25560e1a51a861aa41a3030a616fa8a6e2a978c898e260edc44ab0ed3b2264dff6be0d6da4fa8273f656579bf5d83fe91d5035804191ae4a56b805906c7105" },
                { "te", "d8339bd8b5173b3c653c2192018fc98945bb3d1a338120d5f7f05d6836d8bd2dbbdbcf922240ddc227bb9eda6c4abb43907001d91b0ec0e18cf10d29240801c5" },
                { "th", "bd05bacaae443c8f5fbbb8edfe1c30d5883b1c4d700c6a7b942a7195c95ff66183a5502c497c22ce3962de95e10009a9311d1c32d411390d3814795ba9f659bf" },
                { "tl", "75e04320b565ecd6f83f571be73ea5d5c6886c848f4c8dc9ce63db12836f054f16d67ea7287e3a94bca823576eae66b70c89883f17c66e1ba741a41953d369b8" },
                { "tr", "259d965b2c49ed3e503a7ad226a706da461f7019baddf651791c6f5664fc2aec894b1c9cc5670d0ea7b96f1f718e9e9b8132185ee6b374e01c71569f5b2ecd31" },
                { "trs", "7624e87cca5bcb90b70df337cd5ce6f25a51d47661dfb167142e2852bc921a0f9d02aad0c1ea6ddb8222feb4184cd57e4ba625cd473efb20b287c5d119e301cd" },
                { "uk", "38a1ec7985df6c5f73c0f5e0a557fbdd6151f9136b0fb5867d78d1ef1f95bb07a775defd7005f428e88aeb5efcfbd210014cf7b915e1ce9cc5ae1a4bcf56b9a0" },
                { "ur", "d6bd7c7c4bef41373a306b22566e2d01828be264d4d5ed77a433f6317cddb5ce7d9b05b6d383414dcbd37ecd80a6a3c0e6e7825b999dbe8ef65d2b28b0752ac3" },
                { "uz", "a9258b24ad84ee8dc41402898a0069ca87d87520e8ae864e19fa640d573f2215f547e54ed683480992eef906ec544895ece19a40ad69df4ff7f7d269e3afa8d5" },
                { "vi", "eb76e9ed1fd8f0e48dc081b946f9f350ecf41543547f1ec3fb8b8ad11700fc60daa7427f391ba730372da6ccec39a01471c4ca1342c98cc82a03098ac1737616" },
                { "xh", "bc060f80c1d6962fe93dd785f762d5d12e0ba7c25c215309021b6d5b6134868ba5ea32461eead25da07152161f73f5e25f3a219d1dc6e23f829c46ec59c3fa63" },
                { "zh-CN", "be9aaf3878ed2234e0d92c7dcd4167a2a86ab5147c22872ee3a48c8217eaf1d53ab5e200127fbfde687aac8bba96ca8883932b0ca66132b168cb4cbe561f8c53" },
                { "zh-TW", "d1e4f023d3ac3c5fc6595c7deef69c5ac736bd337d2d682b3a91339f63eedd83f85cc23313d1719e02944ab1c0199f35f065b7152527e9f7ac7ac4972f105d12" }
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
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
