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
        private const string currentVersion = "116.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b6/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "6a45c29f92a15d065bbe6ec06bd87d5ec9f769a91bcb890c1f5c255e8a0e45fd03707420496ccae902d1931897f2986197153b6482a5e32d586901fb47d2b093" },
                { "af", "023567d03e5f9755ac2cd4a0b9317c649129731d1b657c7d56b34634420952206f64d3e71479799bc7917635d7187961ea4edd5a03ffe526cd58d467dbd7af58" },
                { "an", "a026d49bd5d6ee16af1a3e9ef431f4525cd80b9594fd479e096c27ec7a52bef44d3255adb6cfd5b3db3fe2568946080e1c4b53191bec30f3a5c5a23d6c455bbb" },
                { "ar", "383f88cc856847aa70ee30352423820a79d0016d051f97a3f09cd7712627e823a1c7b486b62d8633088971262054ae7fd9062ea3536477ad9e7ca413560a54c2" },
                { "ast", "7d8e549ce3a4f427ad6f145d9203f2d4c7c229d61759b178f93f7340d34e7816ad7ffd1b274c847d3973181d8779015b5fbedd358dbff1d8419e0b6bbe4bb62d" },
                { "az", "02b29362da5f5c194b095ce0f74c7edb1f3807ea015e684c670ee439dd7661232caf2efc915c2ae9e04f67745ee52f50178c9ceca49b58843908ca5af465a562" },
                { "be", "df985c3f53745643c6befd2523481f4f1d679538bcf1dbc97e45fcc1121e1cf653c1eb22af7ffb6d1663dcf9cc5a85f9f6d0aad15dfa2c5a2ac6847a57816d58" },
                { "bg", "58500573457e8fae91ea6819e9c6d61d50f7d160059210ebe5c7d8fcc0fac29f0c9f2efab8a8b7224451a7fe7e5b3d7c13fbabe23a91cb9b8737b4ddb2a8c5f5" },
                { "bn", "a116f5076d6407a976872793db2d738e116b334894413b37301f7c682738b66dda909c0e04abcc42d3002224039519cb89fb53093c00e77579300695d6834005" },
                { "br", "1e9f36e6f5d741953b18d41c39d189935fe74294a7149257ab35cfb94fbdd7efddfd7f520526ab59fcc9318d3200a56b99bed720385810947c1b83101d61df37" },
                { "bs", "1812786faea844fa796f075b38256c2bf9a1981b2a70648eec68f77b57bbdf86096e2f9898bbd674401b3c42fba733f1215d19e2ef9fb122e95ef8935070f7c2" },
                { "ca", "f95eeb53d3ecaca67675a3387e7980a56d1cde1c19a3b317262a91aecea27e2910d6fb36bbca060cf8e956523e44eb166d836136c6ded349d66b4f994d59e549" },
                { "cak", "b6b29f750c95fe28e8c978436065723123798ab8333538bec5e2ba24f25bb40eadfcf99ea43d33a8eba533c7b177fe9671a2711eb1ecccfdf1f1f5b62bdd4daa" },
                { "cs", "e249b379c80577ae3282e56da7b025b8e556c86892970586ef08438fb57af5ede34bba445464098e57064141689ccd93ac2602dc3a6d75f291beb56ecce06f0e" },
                { "cy", "d02fdbc34f23239ee27798138ed2c290c4c6d2e88849fdfe38a706b30809d0fc0cce6c2fc71163831635459142e537eab0537e8da2198f17dba4d3994b5c22ba" },
                { "da", "1535d293c349ee537f5867c871e58d50331ca6674905fbcd2de4c37cb63e2059b903a32e255240ee4e22bd10f2341c3014c4639e7d9b6c459a34bb7e0a05843f" },
                { "de", "0d413820fdc7ed673da8f4678ae1cd29de03c1991b0cea62a47e99c2fa48222de7fa574e0f434034d1c045196a12740142b4da4985bb398401d9dd36378f7282" },
                { "dsb", "476a87991b56b6d1ca453ef4d3c6227c84732975297643a3a5480e7f66dd614a784558b2c7415231d1ad21d728b8149be3486a905bf6dfe71286a1a9c7805637" },
                { "el", "1b8bf2b033b952a387d0502d02b3f7d0093b566cab167a7698d842f7f92260867845a425a7d79f81314f3038729b12fc0d41e7bae99dc6f7bbd05fd68d4c161b" },
                { "en-CA", "0232471f9f1826444df676a5aa336f7a0e916b947d1227240d997a6c4651aa9dfb654396da20a6421f167e0f7eed57341d4181fbac946cbec3b2cc7446c171cc" },
                { "en-GB", "5e7385770b73a41a9238e332169757123286f6656f09741935a4ddc8582cd02e046b2ddd5b7aa40d004f517e9e8b91ac61e3f2d0bb6d974c2cbc09a3dfa3ca93" },
                { "en-US", "02c31ccdc828ab38bfa6a4f3501dff225d8a00e05cfdc57b0cb1b5eca5a4ddbe5b7f23ac4fa130a6a65616a94559533765e039f21e4f3ab188939f9a8bf11f8f" },
                { "eo", "17731be98ab2019f5a5585be266c54694a7a8f7f2f83322105cc2862b152d97a60d2c6da555abdb5636e57d5b5328c16d33b533f6ae4607f674d8c2264c583c4" },
                { "es-AR", "0b30cc50ddb5ad152b756ea3f174c40ac3e5083ff5104f614b148476b113e0b6156c292458671b6df3c59748ac8cd58770dcea729ee809c5ab22879832df7e9d" },
                { "es-CL", "cbbe53cee7922afd84a3b298696d05f8349e1a51962c8c274e485f61f9490bb2ce5cc38d932f3f2cf317d21b5447ad28f0a5b22b353aa48aceda259d788ca7ea" },
                { "es-ES", "ae76450b3947d9062a6ec6c8166caadcb5ca6ba525b01a83cb54d5a5ecd1717f1009daa445f59d5eb7bc40fb8bf49ac6639367c6909ba743cc89f5cfab1f2708" },
                { "es-MX", "aa0402fe91fc6c0b032d11e4b906fece5f6df2e1fb9d3c05e456c5ce8a54a5cab25b3a1d5caf0d9d4fa8a8b5b6dbba899ff3225e232f666e51b414f3936c4b34" },
                { "et", "6cce12dc1f60700e337ac5b8d6dff62555b0f040c7890c7d755739a0c04d55ea367dd396f854c0ac5d455de18275cfe0d21bd40058868e97646886199c3d05ac" },
                { "eu", "f3c4395bf4ce2e5dbb1c981c1827e8b695c723babcdbc7edd0c95e5d00081dc01ab148f6d70a5d635122b27511ced37c0220c27a3bf738e65e1db268b81a4acf" },
                { "fa", "9b9c262c9682f6f81bfb826f791a3c78f15bf442325f110117d2d7e1731e79a064bb81b06143cb55c9d22bf28c6f837a3e3c5f0f8585f00a8b33a09f38a5aad3" },
                { "ff", "fcc09d5c670cda252fa63ace5b03ae0766105ebea1949aadb682cca07115c742451f58cf4bc04009a877a92c12dbeef43757a804befbd97d6da446e2ec92359e" },
                { "fi", "5953f68007e8160fcfc013c3761e90d849ced29ad49eb69812bad80c4b2a71d64232c38c30f6da2e0c6ef9dfb50985945ad3d8eabbce56885545f9e278117e3f" },
                { "fr", "3ec03982efe064e98c08827b741af9f5113c2bf1561f8ff3efdebe8d4e2cdecafab00351bd2709d7b8bb0e19cc18b95ca43daa2d9ca0e043641349b91399ac62" },
                { "fur", "cd1a79bffd837cfe5ed8b26d15ef847622efa1476aaada0e96780a89a961e7b6f8eac1669fb06acf24b6cbab4983c812f0a9a76bfefe4d0658888b0d3abe90c9" },
                { "fy-NL", "f69de6e8f615849f4ceb7cbb9c76024bd2b1d2a59ed5f1aa349e15514d808c768b9fdcf2a7bff7952885c03b3532bef706e157ad3daa59d44bfd3b00861668cc" },
                { "ga-IE", "56fd8c6d84d3e571c723d5db151c33462aafec0dc87fb8539673821dc3702e6cdf18ee7036e18a94f0470bae2162d883b85634551fe303f36b832021da59674d" },
                { "gd", "13646885e410bd18cb7f2261863c5c6642169e1ae5603b17c9994989d36e421da6c2bf818babb2fb1a197ec72d4273f1ff15ba81410dc5791925fafbaa96c000" },
                { "gl", "5d7bb063695ad092d55645781b088f81edd8429a41bedc73cad875c75199705dd5f1edc0646f7e7060bbf4df9419b183e5064ae275662455539099b7ea0e0d77" },
                { "gn", "4badacae20bef6ba0066de78adb974b0bbf8f39832c369198ccb0518fdd4297597f6f349a2b3e5e5fbfc9f127ca7698c2de1ab9e163f9b1a6471a5b4a6ec1724" },
                { "gu-IN", "b65d62d4426ae875fd6bb5353e97f0a235f6da7f80bbda226e1a72977a8300c7ec3d7b8aef7036e3c16b4057b86f059a4316d60380c30235c756c686c17a8219" },
                { "he", "5c97126450745d7fe965a0571f7998662b2c7e2262e5685889773093f85f145d1f881187fa177043db0956e39d25d5819d6bdc573f9e2a6fc02505d97f95e919" },
                { "hi-IN", "e3c3be56f4b12cbb2d83925ba67e6673560a3f3d65943176e9142d9dccf881546d24ca70a6d2e5c660e332313b0bfcc19b1084980b025dd3f16f44551c408a2e" },
                { "hr", "5e31f99f0ab263bc9da80a42684a01a3e095c16f42c356dc4123d5cd303c2f20e4764d05803f5a185031ad4792caf7dd6706c80a916fbb39b0121480a5f1e614" },
                { "hsb", "5b6d66f0d6de10512cd74df2163f0f7d30404100eccfcb22b1290310cbc73e66962339a28a3a4d7a0602acd267aeaca5b7ae2ce50f6b549a8a2903aa577e7945" },
                { "hu", "afaef0d3c979353ca4347dd1357ba4a4eccc0ea473c8edaeb1e54f9bfc58cbbf15e91dbb7a63fa8ddf5171d618995c642f55e43d2d2f3ae4c2d14c5e3ca988cb" },
                { "hy-AM", "d8b751dd7221c81cf3a4a2cec40ac7219d2688844e073d508786e4248396bfb7f776ff2429c24a52296ae2848c9fe256532e2916db720b7107c437bdee019f66" },
                { "ia", "59e99d76881a95f2bc7486d05c2b38752105bd212bd26eb9f4adf658c06197e2a92ee7883e700922ab034e7d0ca36a37a2ce26035c8a0e09dd90faa5dc315dab" },
                { "id", "b919a5865104eabaa50ea1f308ea2681603e7ac5da635905af3bc35cfe0b7cce2860989c46de6219d3bb5da2ac2c8d7ffba2aabc5a1296ece5473023bf91111c" },
                { "is", "8b1a8c30fe4b093ca90e0957d8e7819ad8a57086719a3073f8e450230867f16200e6d3d02093bacd8b2fae3b6f8781f3aca7708f1a8d5be7ffe37afc2c4c2782" },
                { "it", "3c071c238c9f6bad8a97b2af657be946416ea010e9fefb58bf81bd98784a609a048c76744bc6a433a3d080b0187ea2c7678d7eff7349c4c400835fe0f408bb72" },
                { "ja", "77bdf11d11f6b894c6855779c9eb418a4849bc40d23c0de840d8adc23bb083716a3fcf58e25995aa7b847aac68f7539998a5471e161c9327047ee0e4ecbbcabc" },
                { "ka", "41b8aa062395ea477c4835f9988b3f3f9370ad6ec2505ed70de42352d9349d085f700d1336552eb6da5bc72239afe88e1e0cdd47a24eae8576e5a8471c052a4c" },
                { "kab", "7d65d6bfe8e9085a611952f3f6edc90b39ee55b276a5d5040754e519af50955e2b20bec5877608ce66d9d0ba1fe7b01e7b0a6c59de1e8fc362dae2bac18d359d" },
                { "kk", "c818a15e3ac4b8b20ef5bb2c8450a64e1e3c12be2c7873e11fe543a9304bfacff3ca7e373e50226d44a8179afa5b3fc7c0e484611bbd00d6267afaffd94ba2f9" },
                { "km", "9f802fba050ceee660d0c86d3806a8469f2b2adf9a2d76d4862485749a2be3e057b30e2e99b5d07d8fbb6a810b690bd5accfbd4c8be9e0bfea2c3791075fe849" },
                { "kn", "59050b98e5cc16eb4502e2a299747333d329269966c1d59ca99ba4713214af69507ffceed50fc0c00859d05253470417f4fbd1e8d9c2b4a93fbef08a63bcf29f" },
                { "ko", "2059621513ffabd9ba370728f26db6423f441a490bc284b2e64e3868c8c22c4e3c194319c8aced1b336dd40be321b001611771488568cf9cf73319b2f34aa5f1" },
                { "lij", "374eeb75dd8f865f6afdab913048acef77cf890d5c4f88ca323999465ddc0bffd2bfbb0d6c3adcb7830d6cd0c052c00d48163c4103b65876e6e7059d54a98819" },
                { "lt", "370ece779736b1ab4bf1c579f214e6889ca0b47dc1d2c20e1b336ad3d64230aeaa4430eb4ecf1b803b5ba36caf42c58b57b01f148472939a794d15807eb3a764" },
                { "lv", "a4ca63a57976cd347aed81e03ad885682461af3ae2d537622fb4113e46b7a70470da511a0ee193536c88d5f0dac5afc332e3cb7b105aa52e008eb808de9b0232" },
                { "mk", "8ec4a808ee5fce2bed83351bbd6377c0d1ca1c18e1e1a387fcc42fce48d88fdee45153d360f6b80015f873fa9931db2eef05e30a94d6a483ad1f105edfc46369" },
                { "mr", "aced7a563de2d43a57c70c11cf7b94cca691c74ad01a3114623abedfcd025c61d554346b02a49ac53ccac55351d05c9b8b41499e33917467608d3bfe62a631d9" },
                { "ms", "73f1b5ed1058cc9805873880c3ebb5a1996911c93cca9bad2323d0fc89643fc834d47b0a020f5cac41a488a426a01abe718f08cecc9435e2559266f0daccc9c2" },
                { "my", "392e548e86d979dce6aba005fcb430df7354fd0aa5f42556bea3d9875e40fa64b59ac353b60b121a74f347953f3a2c8c7f7e195bbdaa9548bfdb5abaf416aaf3" },
                { "nb-NO", "6d1591b994d9586e60a9d7811dc420c5f5871c3940460f3a256090f93b43779e7d3933819b616e20f008544663933dc4e3e127790f6bed231477a0146961ab7b" },
                { "ne-NP", "43183ac5a6c2ca05f05e71d6cc07ea83d92891f5557e151c4d5c9e866220c0a8bea8e5994cfcd8a082d36785b20c7a7929e8714cae9b3eb79f88e9a1f8c8259c" },
                { "nl", "67bf8204183041c7bfac80aae605f12514382ebe85390b8afce637df7765f8c1888632bd839a0cbca121155d7ada1151854a4f112eead7049e1bdd427f02d3c5" },
                { "nn-NO", "2e407fc0832e1ea481d3feae1b23a31fe241682219dfd147e4bd0a3557c838876e30a05af32724daedd145b381ad195b854ca39274c623a6198daac25457e39b" },
                { "oc", "de6496fda167da7f5de673c42b0dcc86e4a42f6bc388188b50a292dee71acb781d6bb341a741f8d7cd8ac0d147960bd80d424abdf7177f05a420766f04d6061d" },
                { "pa-IN", "07c79f2ccdb87d09595fde554886711ed892af861cfd06204870c22c1bcf686279cfbb92181f7c48538141d24ce2b84230d1eab726b9016a2bee0b717e57b215" },
                { "pl", "df01f441f4c5276cc38d67ba5b6a8ca00c00979c1d86df2b30cf7a35c7f23012bf983cd1d4300738f0ce02cec379de140f0a8c308cd0f992b5b9a8d8d4956f6c" },
                { "pt-BR", "a6f49fdc039fa9c1bcb609a6ceced83e8e89ad153a53191f27632ae590cff66ad11e59986f329966d4069124132788edcbf42a3753b895f705f5eee10e3f3be2" },
                { "pt-PT", "a2cf6cf69011ef7cdf3baa68e6a746bd1136eb16a5cdccdf372673ceee6a2167602807c4946eddfa81d1c1cf652710d38546740e75635391ebdb2b013a441576" },
                { "rm", "f12eec0c3a2db5afe7515e81aae5dab6d56567252c319364042b23562fecb757df3673b523e0e9ea17ba183be09c96506279c8bc303da51749d427bced21ffe7" },
                { "ro", "7d8d7cbd4a1aad19ea6fc0ecfe403d8de09d4556e750c8733cb5bb8fcada5ad8aa1013c30ee44e6fcd3b89796b028cb931276b88d9178101af22208df39cd07e" },
                { "ru", "2f36af75a9ba6c522d6aa41d333f31ac9399f8ed6eb7e720fb8ead7774e6f95f7c64c4ebe4dea5df10d7dd8ae0cb33b95c3c458d74ea2c5be841c2dab88eb47e" },
                { "sc", "bacabab4cdc31543f9541997e1ffbf8e9c6125b1e23f7ae96b545392fa8a2eaa5eb1e1979eebb187b7b9d3d6b013ae36ed58ae58ad4bb99a97125fdfdc9e41fe" },
                { "sco", "2fc10984b436ac96b04406178a8c7f39773e48b4b71a07be8cb6a723b936cda146fcd77a2e62ac27088712dc73545e42109f95dd27b4736b8dc1918f7e1ac924" },
                { "si", "53943036ce811d2523e086d130730d8d41055481db817d37b8cb349d373e84b1aef4ad233230bea0699b6f28536c60aab93625a88af99c72dd101ac5386165cd" },
                { "sk", "6c8c4d3d56643aae0c1634fc150f89cbe6a002b2547dd2bc8335dd277ebf683c265323d4786906123bf2e50fd2f801c0b2e9f5477b476db19ab7c93df3910e15" },
                { "sl", "dbcb9e0d1c986365b0697f0ab3300110ea5c59fc800909c53d2ff0fcc7ee47c58abe5bff19f384409fe83ac801901ec31c6973c05eb0f0844c5ec2cb76515c88" },
                { "son", "ddac19854cf134271eaa33d2481f627f1f5ff1975e3590798623df93735bc3553bf2040bced4fbd1d70f0d53a5a65ccab9375457c46a3410ce19798ce9983eee" },
                { "sq", "76f41cfcdc802da02d66d776b83feb54676748fc59d475d46b7454b89a6c8fe6370e9351a9b9eda033d1f2e9628646a9484c4edd8c333df51e27a8bb9324a4b5" },
                { "sr", "9460a5c61de187f90f05de7149d73ac716a89c48731adff3ae730141023b20b1401edb44c765969a3b00d6bb3423612b3a07229d45767dacbcaa304160fa78e5" },
                { "sv-SE", "25a357e8a95fc1f2b7eb503aab4eb82940a6cbeadde5f0b663b25b8c25eb9cf6944e51d186829e63ea0a24b47034d069ea63630b58f11bf958af86c7f12e5103" },
                { "szl", "b8f15d65209912b2403c39568e8aed734b101e1ceb4279ba79b6e100609b4022f3f038974d39e70cef535757f65f6dd4f128d15c6ba77f2b02abf6dd63741557" },
                { "ta", "6e870aa1bb66cfb0e0f413d0462ae0ee5b8b216dc22b747f38f8084bc4c9ce41fc5cf602509b06331e17603295566469b5b66d8d2e498185cef7e47139c944e4" },
                { "te", "4ff37c1200a05d21cf58ba06910be89ed822e5e81def9c2e381dfb4c460c42a827518a62137e34a0e59b4e0e03f446ae318f95105cea1a633d6558b9cc536925" },
                { "tg", "dacbf6e88a3dd7c80115cec7013c4290668814bad724df320a5d7f851d29b0332b996542f2127a89e1814286c36fd200254dfd2d9d7415376a583c5d7811e7f3" },
                { "th", "b8b5ab747779cd4d7ef91de1744db68689aa09122fe5ea296bb85126f06b124ff4d0dfc8341db9314f506c444a3ec40b415f43e0d81684cbfa22a1d729025a98" },
                { "tl", "70f77a58dec72aa2e217988a947c54dfc67b31d74d8e3a49a3f4cc709d5bba32e475eaf1497ed5316d508d1d0102ee073423345cf0d8777995df196e3c4e9f99" },
                { "tr", "87405244136f38a47673574d41475ddcd7dc8c71e19a23accc05857c73fc8e1ec4fb7ccb42e176585df7a25cd049ce33713e3551a80d47f495a456747968ade6" },
                { "trs", "17a287dc8a0b0cd8e6fb02f274ba893b77dfa538364d11a7cdd20a7213620ca4b3a463987f63220a08c43e681c4c301f4fe9b386494331457d2ef508df64c569" },
                { "uk", "b340272c5123b75f4dc74306d27fc1c021e3c9ae98c3c999887bcbd75497761ad4f155872326357ddb3c4d59c3c9917f0461e61acc8237ec98a57b1e1a9a66d1" },
                { "ur", "655de1884d5051f23e6bbc460781005705fa1714c2ba02f0699e1a0db61ab1993caad40501ccb7563d9aa99632a9b9207902c4f70ba2793e7fde2dfaa01f8a04" },
                { "uz", "1253ddb9279faaa287d537871a644968711797aef7ecdb25b6cb87eec7d5048b08c9a96c3758cec2c6ae6f5d2648a6cfb1a615df95caa7ad6be23badeff7e8d1" },
                { "vi", "9438d6e1724dc4ee4cca99c2512f61a558e3a8f845e3bd5904948fec9e26e3d84ccb20ee92124fb623a2d80c075e4aaba02179289c83a14deac35d63416580b2" },
                { "xh", "fc17cf38d7a84b4277e798acb57225a10a12a58cff64145c6b8aebf79722915277a0cb40fa4b196d9f41ecddbad99432d89f320961dd8340e6ae0412e255bbeb" },
                { "zh-CN", "a400fa2738f43b115342964f18e744ada9dbc469f93e08368912a55e14f714474e32930aea591d1f752cd1061db850ad1888b4b2d3c7fd4a97233089124daa89" },
                { "zh-TW", "1692f58813e953e5077fc1cf8eb4a10ee6693190b736e9370f111e0db06dc00fe533bf5bef9a69a5d00ba1b2daed10311bddc0f0e5a792be83c265a22104e207" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b6/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "a8ceef54b32793b4f137f7beadb26ffbc4e171f8b87361cd9c7ef6bda8e98100295f5add23fd2c356109bf7e5b57d57fb1a1615e99127c41fcdda870faac1f33" },
                { "af", "c8f36b8cdfa238388ef18d0b3c7034aadd452158a9fc81df8e7c89d63ffc7bb11844874a16f691c3147f62834bfd4650b62ec6956a1d3cd19721c5bda7d66022" },
                { "an", "ec70393431ad58e6a6b3902be4624428a4646adff6f9f23c7bca9f7c7e1113d613d99f88ec599f99c4188edefe3cf5df802315f5ffea153390d840b93408b5a4" },
                { "ar", "7cbeb3ac0e9a8bb24a0c6c158546fb4ae8aa37aee8da57b258093b17daa643662ea6b73720b014ac893eabb46723f084af9b117e4f964dfca7781caf94edca0d" },
                { "ast", "4f23a0248db2788045fc1c62780b34bf193aea8b5b87338bcb429e71eeafea92fd8dc2208823e188e68afca5da7ff5057e0783194c243d604f14074b821b890e" },
                { "az", "02554a80c91f5882f0c4bd3eb69e946ef10f62397199f7561a56b360f848e67a557bef3191fd4c62a1e77692d7a532800bedd79e12ae9380eb7ac109e17add84" },
                { "be", "3c670f60caa3223bcfc5039b56a3ce157e5f2c2b9af854872c192ce3a6e8b282c67c294a6af2c69e223472c6f59b22fb34ad8a9f4a6a1cb57de29eeff2f16c85" },
                { "bg", "e3d69d54fce5be1257aaba45eeb3ec9841465dd11ebf84bc63100ba287a9926dbb6ef66ca416cd565b501a8001b8c6e474fb56f9c6f45a9586f61a1895988ba5" },
                { "bn", "aaee874a33e90126addda73d8ef87534276e1e2ef157c2933b4cc0906d03f915027a832d6c6c4f5bd05d24f2c01e751263491d2b5f64584956b991fe5fa6f47d" },
                { "br", "82fa4a277d31c7f8c8533860773f4307bf0297f939b5490b74215fcc50d6bb911feeb321803b89599bc063f43a04b8d5092b15a21bbb2c598d9c0a0dbc511260" },
                { "bs", "35d86c46b557297f80f04e1d15a6b5746bceca80cd94806008ceda369d2848bf89111ea8a56f467270d7116408a6718d1f1f74ef72911ffc8f0f4db926a53cf3" },
                { "ca", "b01d0f3c6feb9eca6e961264a5d94155fc9d785313688e1b1f221de4a2377e5d5613bfdadddcbbbe1b026be56a42f91e2fd8dbc3f3b1a4bd5388b5c2a7fdae04" },
                { "cak", "7c5854f8c312021d8bdd785729f50d4018d892a7117f56fad90be710ef02c8e2910bf557a09a8f8a5104087b97820f0122712dc0d3d0d3544ea8b2812345e611" },
                { "cs", "0a102b0e65868ae90bd25df0f6c66093e9f24fb231eb39946c269cbb1aa4a06be2033f44a4b8ad80e3a540522b6c06842125a9cfbbff693bbb182c24065715d1" },
                { "cy", "634d951d2a7149b08682d998cb9e0069096126067a70fe5529db7421990aa419a83eb58733dc83ce2bd1d06489b5f4033165910d7800418089d6b2cfaa88fa5c" },
                { "da", "6dc72119896fa1e1b9aaaaa0630734ed53c700621353c5f64a9ab87324fd94e4dae1fe79683be02681ca025803063c7b3c53ae260c6870f2ddc6d9c418c0476b" },
                { "de", "0ae84b1ed5423f24da36fd197b98b41f580d331df55f2753b2e343cc6564b21049ed04677ff52a6c35bb7fd2df121d70979d9e68a5b4816049c7c907d3045e60" },
                { "dsb", "839fda8aff0ac7f025c5a05e612b1edd0e83a1db4e1eba5dfa03afa04808a5633c854cea185b782aafcd6d851cd261521dc649a9b1f4e44175dd80ad7aab410b" },
                { "el", "7d91c0293f85ccc2298b59e19aa0c655021d7f0249dec38e1381223a52ef0b7c7a2ce26fbba60518cac0ed7253069458b854f51243ce2472fbae01e1740c9375" },
                { "en-CA", "89c7a9142be70c4fa888106e00d0dd0c4fe5c2146e3c0d728795ffa45962fd6faaa550e4e4db6da21476e7fd549667c01eae6f7627a40a686ebdceea4b7b0614" },
                { "en-GB", "de8e34bbd1ffe0bfdb0fc9150d380be8ca9bbcc73df34b267add030fceda3f10bdf0636e95cfbf5cf2b5f667fbaffe42ab2e06638618434cd39554bc19e0aa83" },
                { "en-US", "6f58798a17e34f3dd1613eb2a3ef47703700d784a0a6337b72a245ac7cc210c72ff9083fa4d437f02b5c27b02834228743faa4d57b5f92046235e843c902717d" },
                { "eo", "b90a26a78bb0b79e60f44054ef50715aa2c3f21f9e9faf7fdcb8104bfb1e9bb9b4b083600cb40844fdd600a53a6805cba61ee250e9ca10421f4986b22404e1a3" },
                { "es-AR", "4c96a7722668aafc15c3f16d3c093ae5f18f9a507010438388a7a98ee6d337fac8b0b4b8cdc3f52234d062c5f4352d01975b3f09840b26d746613d41de1f243d" },
                { "es-CL", "97a753aa8b97ef990fcd9b2248de6216e368e17b1e255eabb8def7e8e2ef4434f75477361e635f23e5e866d623b4d62e759bc7a2b6eaef609f540d0d3aa572d8" },
                { "es-ES", "c6401ea830002a8df6ffe91024345624f164da2029bbacc308819f204f9ace7793edd0cce253132657a37fc7c856475f9386d30e5def17bf5fe07b289ce27ceb" },
                { "es-MX", "7095947d42a6dd20c0a4c5225ab757a009100ed89d1d2cc01c99d92f7313729958398ce195f535b0988696304b3a1737454f79a76ab725098756d28e3dd85e3d" },
                { "et", "7cf1aa5a625f92c5a32855469ffce1d730549a58cefa8a0728b31aece9f88c9a7549cffc39eb2f2ed13e4acb58181cba91d183823de7b07c8b61cbbb4b64bcbb" },
                { "eu", "87e52ff29aff638fe099f4bcd0416b42353f98746b0c36b161c63bde36e66dd2956850a16e1cf40c57fc6baca37a9e5ee78470445338560e6df4c741d4d52147" },
                { "fa", "c31ea3d60b506bf59fae9aa203ac0180f3a07997c54bb02be49ff1e6063e9c5457d71b461d20d9064015a1aab719ec5d6849515883e43f6ec88585b3dcd2f41e" },
                { "ff", "5ddded88522744dd6c745f36096e54395efe1c62401cb5ed0c6ab605b4f81477b284e5c7a9bb6f4789b050a21abfbebf61ab9268bd71bc37b6d53a4de68fb257" },
                { "fi", "c0d8b2e031cae292f863353acc227fff0fe91a6da1120ab673906b3fe78e3249a3876dccb42bfe1b82e4bf27b1acdbcb45ffcb0e2ba039646ee71535190a3a3c" },
                { "fr", "706761cd660fb92cb090f9a8daebf98b4038d2cf1854d5b4e5308edb4ac0a607959d70299dcafaf253cda8f7c66f28763effb99acd44f22dade034facabbade4" },
                { "fur", "789c52af482dc1d4e1c8c326e89e82de42408b81e6b399c04f868c3681c88a669a6cb9b6a2bfe4f094ae7d3b2e4a537e32ace9ace909c1ef4925e06fccf41529" },
                { "fy-NL", "1c63ff68b19b24b0e3ab9ae057113fe2dbccf23af3e01068e6fce134a19c21372dbcab6e9edc65cdc6530351f307090c4028511522e5edd983d1ab2617be5b48" },
                { "ga-IE", "1682b645e6c8c1d947827593c910f4b176ebd31fd990521fff51b454f9f05ef8f5a36b97b0f67bc3a54a6b35386dc48329c4b3c33c8ce8064c7cde0cc134babd" },
                { "gd", "5324a00de7c2ab47cebc1512be851dc7a30f1a3a7cf3f99597b82b6796088e9db9cba8c65b9d56625d3ba00120d18238c45c07d79f299516e2258799f2a9eb02" },
                { "gl", "6cb2519cb7313c7a796e6eb0a8b95d23c78d4dc8a9543ba7cdbc80ef6d1d3db198a91d6d0eed8d77971f952fd9bab287fa367978773edd48b398fa27ce0ab4db" },
                { "gn", "4a4dfe9bf78f84bfd576b3dbdf0fbf4d84b13f832159d9b49bd2c94ebeb0ce1233109d4fe86397609e82b28ef2393f9605ee4b4f38524d259be61824f60919a5" },
                { "gu-IN", "d2145d2935b86a23dae0e1d418e8acb7eba6512180325b047b049b01a2d911f41a42f5e50d3b59f85af0427ef0b28fe08bf1f1663db9687a6c62d2ea7e08314f" },
                { "he", "aa3829c75c00a510d6a71f43efdff2c3f35377cb8b21c6e3ee5cac7a4dade94845d9062e96c9236e83f8f6fd4f0db15a7a6b05dd4a808895a7555407258a876f" },
                { "hi-IN", "0cf98084dd0b1d935084a0e1c08ba837b3477972c1b229371b5fe7922b9191f475bdee9ee8f9ca0e98afc290eb4dffba3f9dd74df86f7abc50da6b35ef03e4d0" },
                { "hr", "b16941d6779c18f2c290ca3b20bd7d9387b224e5d40b7683a4a01ffaef4aa5bc16aa2877db7c520cccd5758dbe482b3f5f71a30a09bfa7553888a2c22e2fcda5" },
                { "hsb", "54d70a68cc1de04b130bb512c57ca726e1de85365e4a58cf27458606908ae92ac2596000d6729325fa1043acf49a347ccc9d0ebf486ccf3d7b309ddff5ba8391" },
                { "hu", "3eb6db2312d7dd94dffd3a1ecf4b402f08fa2c9d3944ff080d80d8455bec49322ff32960564f981975014279a71f0e665971a8389c387d675cc5f5daf119f0af" },
                { "hy-AM", "acc5022183ca6e710fa88bcb3d02f5f17889a97fd4605578c3377d28d1fd02a613d7342240b843a0d339548df5c6c002670401cdcb6765d4966900ec147d312b" },
                { "ia", "707743ffb372ae8a252655ad896fb125754f47b11aa8a2ac50bf7391a7f0ebcfda0e4896fcf5236f4ff586faa5333a708d187ddf380db10202a07df5fe9db7d6" },
                { "id", "d1dac4e8ea0aab6b75bccdd0466cbf09f94c42a0b0e08aa180f2082316ac93b98d98b9b058f6ba6e21f7a478da2b30456893eb73211933b00032142e9c2415e6" },
                { "is", "604e6bd7bd35c5d8b74f302bdb968df13972695c539d8184afc8c2722c795b5f4ba745a6754c499e7cfe15e0787f8b4ddec22229f1f0ddf5aaa867aafb11e012" },
                { "it", "c0d622d4e681aa3342c4784837890ab0564ed0e87aa8743e2dbc14020564f861e6ce0ccb59b793dcf6ef3a044c19d084f3acbb6b0547416800c2c75605883b10" },
                { "ja", "aea2fdfcf153fbc2c6f4d4714ace122f5ace8bc6156b6f25b6d51c8d4eb0b66e4fb80acdb434c825cc22121df1cc85bcffbd2b4deaaf2ab373a5e6334a3f1ace" },
                { "ka", "ea68b6f8b29391503dbed4d25bda7b6460a92dedc2de61472ce6fc1bd7c54601210b2ae2d94d24d2ff1f5737399e35a64d919d91a04d3a8db26c6e170a49f742" },
                { "kab", "b35e517323f5410d83add7fefa87207f034b156e1eb12c2680cb38c83e0f187cacf2c755ef7a1f6979a0ad6b8a8dd726beae876140938f30a2a834daa6ec8254" },
                { "kk", "1a5846bbd4d46a77448ef4ac08ecbcb3979675845677643c8e0f4f50742f659507f2e6c7ad7c5ef8e1aa525230f74a378106411deb8e82e2a6d368b3270f0197" },
                { "km", "35f19fc1a00a68ce0c0ccdc88f907f60c9485b611727b9b4845f7e98cd640f734b1473db579415b3486b0c73cb9b107eed563f15681c1846b47726b91bdf9d4c" },
                { "kn", "07659874bd9a56b4659e7a3f03f4a0dd24e01e8f3e6ab780b80a7b9e839ac4e83126e2942e15f085a8c471dd02e965092c6e75370030ee6c5d97e7e2547c9eb9" },
                { "ko", "565e71bd57db9e97d40d328b24f802130ae3aa31cc346fe4fc94398041820c6835c21be8eff1963fdde6467999da033eb4975a8f24fa806bac78946c9052cea0" },
                { "lij", "5e6896aacbc0f4e9034d4c23b9be996c20532c9b0f211a7618fe61611da876be2f91e46bb2ee574b172be0c49388ace2a6c839c6b0677836fe1b9df5fc8aa16d" },
                { "lt", "73ca80a1b0b71406bb41af5f4821728d3731575c166596c8857134d68c6b3b24eae9cec4069753d0fddfb24ccb50c8b85e89294c2e9d5d72b4616580e64699ac" },
                { "lv", "36285297ad44c0811263879287f74774f1be968f49cbd119db7b8bbf222c0c31c29bacd740b9cac01035716ceeceeee6022efe5cd1c680145ad04986ce00b753" },
                { "mk", "1f811437ad0680aba768196e7434d14981bad0449150d2b2a565d63046bd1f9059d42a8830e31f64a7d4c2271587ab2897f5dc8b6518f28a91a1bc1414e804f2" },
                { "mr", "92371eb678b47f9ada68a7500cbcacfdbf7bcd289a17463b9bc3d9cf5613e6755163792c6e4ef2aa3511ea8f8fd8d97b6326883906a5c9ebc61303e4ac7209c8" },
                { "ms", "25845d9f5bc0ce627d62a4c93b0460404f1286208845414a3be2656d39a63b1cd03f8c590cf09ed99d2338565a163570f6e6cd0bd927e2c39c5ed22404493524" },
                { "my", "eaedd264159f9db0e1dc2bb0f690cdc91ceddc75c9e72e5b3ef4e07e66a8b46a8e0d0d69badaf4034ff6457068860e2c51ef26c45cd8574317d48a916db3b34f" },
                { "nb-NO", "615da154dd145696e86dec298cab00c00dd07327632d2690c84d8125f50ee6e5bf462e4940d47cbd18f3582565e6ef131ebfbb0943071b81c55d0e43bee2c864" },
                { "ne-NP", "e5b3eb6db9e9b156c0856ffdb11d3493a897767858fce204cfc8bc7084e44ad61728bd8db3d53e727df19be06c4dbf1bb0fbfa83d34134b042a0c4748a4cdbde" },
                { "nl", "6b907caf56c015dc7d1c27d991d0506538d570c66c1ff139f49bc3a19febb2a58796141d62b6c6debc37b098029fd0210058416a27fdf16c382e8133ddce3471" },
                { "nn-NO", "a46f64669d3fd525022217be0fb81a969a10e56218cdd0759e7fd9d80b7ba353f158e71fb811257cc110783c30da69375df3dc9ac0aee8dcd37f9ff07cef7413" },
                { "oc", "119ffa311447f4cdd29ee6da1aebf33e38c9c137cdc188195ce0596c1a696196403febb9c0ed40b87a94e83e80372b0d3a66b99c971d353ee4733553d6059633" },
                { "pa-IN", "1923a46ca810dbe6159ba0c0de2b739f6708cfb85c72d3c5df37e8948ea007ec411275869a08a8c4a9e65fdc9e3bbdb28005ef14d7e5c638e16a41f462740a6b" },
                { "pl", "639a30e7bcaae977cac1dbbacda1ba47e3adc0f07fd82c5a60986397c2bbc94a87e3d4e2f581f477439d5a99fc9520ac16e0bc77a48b181c0389ec6918303776" },
                { "pt-BR", "18e3436c47e891553339d9e1509bce984aa8f598515ba65fc2030ac257584f5cebbe35d927bb1d96c3c022c5fbb94176b15a1771e6b2889098bb5452390b9435" },
                { "pt-PT", "8b441fae675147522349f78f66bc44d9789a34af3a0edeb5335355b4bb56d81aedb1a01455201b49dc07dbcac01a2c5baedf4f6c4abc4ae626ee2e229d191d17" },
                { "rm", "26e0f7519704f9efa7159155d3fd98725b8bf0a8667dff13e842be6e0fff2ef21592f9ee0cf4accf00aabc899126702e14afd4b5c4276bc2dc0d2521ec3682df" },
                { "ro", "1cdf675a31055c80b685a7e055942b58ab721b359cf84e1a960a39ec4773ac5be21dae4a3228665f10fb9e34d4aae303b46049eb769145b0da520248f6cc82e8" },
                { "ru", "a9edf08b0ba9c464c0f1595820f1322174625f948c7a3651af8e4be7c295afd46fc4f8dfc9a735c7f7a27cef8fa166139c29eb0105d88ab455f0b05af668b690" },
                { "sc", "76bc355338961fdb69a99c0cc2bf77c04fa202354ae6084301bad17eeb2f5df1c5a42fb687bda81dbe1f5226f4e27dcd149e7fecbe33c0df9c31ec71be67e1a1" },
                { "sco", "9d931b0a242c4070330c530d5349ef2861ad7eaab30ca84ad18dd128594a0297d722232dedc95c76baaa12078a07b37fa95012d82ef17b6891d560a56277c762" },
                { "si", "8c7a0c703a17f21b909125217cbcf4a41f9feb245dde939f0554e9cf3ca6ed748daed06172ca9d8edd3892ab7e61523f7ee2c7acbc5670b0dee5f1122fdb9ec5" },
                { "sk", "fe02fc7edf7b53f9e5c713a6d99bfb7073ff623c49f04ce4fc5cc34dede8c421f69385c98b4bb4af7c0defa8edfb5f20cbd756546d2e501774533d5a8fa9df58" },
                { "sl", "47742ae4e6847ad622997cbf7fa8ec6d3b6d5ee821dbd0f0249120422a0d8f9d39e63491a798000eebb20d062920ce8a5f2a0c07776a9e10a2fc1316a1bef4e3" },
                { "son", "7c1090ff77c908650daae2c1c0e1ba2326a7bde54ad2ee79aaf18b208c4a8d2ec6c0e16208ffe2527acc9d92d2a259c2733ef6026f11d60c89fdd7de585ddd64" },
                { "sq", "8a1ff396c9a4f9ed6838becd0198bf61e6dd54a94913f6b1f700cabf47a0f870384cb9f2c99965099486dfb985eec70ece2051dbf64f04d620755d64b40f9e81" },
                { "sr", "08ba8f65ec44ad5ae0f6ff5a7519e12f0c6b57e7f68d55cdab8716d591ba75fc18eef582cd879031c43d4ec4ce9d5ebc9d433bd12ccf1cbeb0f4f544ef413073" },
                { "sv-SE", "e917b72136bce652f7a5c82c9bec9d2f1821e4acdc683899ab1de6344f750b6fa447bd6d9fbebf5bb64dd2d822f0ee48011fbba9de0b1ff09b7ec34890725500" },
                { "szl", "4c771c2aee6741a42902b24a458a1219903578682f4b69f45b538f085ef181563e7d278387e8fafb0fbfa06a12cfb02d8a6a730c152fd4159826c8f40a6741b9" },
                { "ta", "0a0dae12aef86a73e8fb2073368046d662773708aa55e8ee5f3185aa0c35455c7cda7e2df0afbd7d05cbcaf14598ef3961a9bf65872e1b12998b26377f21d076" },
                { "te", "553749f8d9b7db508cc982d683215443025250f1f7a8802556ca4f712ec9fa0560fa7187cd84e345c420eee1786baac89c23405763980fb7c2cc6f75d14169e4" },
                { "tg", "02c2f56d06f98794f7b29b9d48c8d47c565027d5b64d39741a1e8664156a59558156a20fe5fddb25391396343b16998d3094e0e23a6c6dcdced3925baa0e68fa" },
                { "th", "50884f6fa78eeb2d0894fe5cf1b92e581f8870593dd607dbbc3f899c76f499dda22594e45895893ee13771562bca89af3e05696d5b951a030881bced49febdac" },
                { "tl", "fe3c1ae89516d7efc34f243bea63c2e4ce49f13dda814aef3b09743960ea159e7696f88af0e00400e78f1793a43306256855a64e7d15fe5b7a74a12afec6bae6" },
                { "tr", "d6063442fc222160f902db08eb42bcdda9d6000424ed22ff2c5c08701805f208afedc109785c58cbb441227094bb4eab7a0a707890453354f327f1c4c31c0eec" },
                { "trs", "a0f5254eae1a8eccd1909be7d2d82d7570be727bca7992c7fd2483962b77029bfd4214abab8568f755b375bc1385521098ac23c0b1c6b3ddcbe9eafb65f5aed2" },
                { "uk", "fd5f26bb84d635e78391fde2486fe5f65cc46edf139d401745fc33d3741fa684fbe2781f70bc5599c42712f22648128f0c7533d372d1600d9532ed65edac6d63" },
                { "ur", "27ab314cf095cb2f73b68d035155faf5d869b58e9249c02f0ba0354e68a52b8fbd1cd2545a4a1629690efc81e8092881f4b9e19356dfb04734b24a059c0670c0" },
                { "uz", "b9b17677a64a02895896f19c342f840b2a20a2b8d2d4677e9338b17b43f7ffdc045fd8398aa329b08db6b4dc8da2db26b7d17cfef969b9d92d510623e130affe" },
                { "vi", "b322c126284587bde8af922812ce8a63fe1165c89d378f580e52a655c89a036382d3634fed5a2f5f78d06e515b8a6f411af3e5b2a6ef1cc599f75c46eb213fd4" },
                { "xh", "2aa7ff0c251cac97bdbdf1d6773690ed07756141f3d061c2c0288900541bb4a2a3402639e6decdb07577ee80736298c8cc0eb0c650cbf54021a11505909db4cd" },
                { "zh-CN", "e6ef8cc026119a69ba51c663722ca09305c25ceda6561ccfe6cbebe9a9466548479aa5bffa664ca2b0a0ba4ce162ba699343234f419959106aabba7c7fa5d756" },
                { "zh-TW", "7ac081132e21734e4168465027f5cddd06154b102f85a78aa14b84a78372ff9bcfdc222da2f68ea52b686135925e4e400293543108b517c949ba4ee4663fd16a" }
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
