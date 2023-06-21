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
        private const string currentVersion = "115.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "07a1fdabcd38e6856c517d594831efd4c1c0e4f69a9e5d1529f90d5538133eb316c933e6dbed6d80c50a6d22a9847b1df27ebc79b34bfe06d21c23196a7e4546" },
                { "af", "97a121deff3837e477c6011593e257846573044f71dcec5efd44bb238225f66a3c5f9a5544bee0a9471337738f9d4af821db541452b5c499f02052dc0b3c997c" },
                { "an", "51c0926c2f0d77b5b1da3a967a651068aae66847e38abf5ea0fa1be2df5ffe76dd1264f0ea98884ec5fc754c0d77141a4afefadab1118f48ec25a1fa052cd453" },
                { "ar", "94a2ade2170e60b6053ecd44a0a555eb02ff812dbd330e9674700a394cb9b3d81f5ba220920e7974fc9fb3fd13cff9dbbd1cf4e2cc046501ad5fab22b3201908" },
                { "ast", "f0a7273630933aec2c181904e1385211e179a8b9ef5ae33a4469d2eb3f0922f78afec568d847969442152e7ba94f3d29b98ab663b83f4ef2089dcb7203ed73a7" },
                { "az", "32ec596f8ece633d4b2d71147374c98117f18ebe3a1337ca54bc1e18a06fc7880872c8cb5054496d755835ad32aeeecdbea6653a4c32da4b6475ffbb7fff2a26" },
                { "be", "4547629fa498e7a065d5945059227855a7bd6de0ac999d32a19abbdc33248b1214239120c37b2565809f65bdfe6c97f77f11707f7f4105767f91581555fb4940" },
                { "bg", "0cebd266ac823aed353371fb5cc91e2117fa94f9a912c6408f187e7808e023b2f5a13bc874e6e35d4f4e43969294c8833e9162ba75f6a4d2a678483dbfde1fe6" },
                { "bn", "20bc63a2335e8e1fb9d1ad7582ac69974d2d6e5d98b73eff890faffc7e0114369f0092ac8a36fd1baa1a6992f4a537ea503840676ffac903416c05442b5cfcfd" },
                { "br", "be7f2d0bcd910708f0b22dd7cdd4d3a367d546d90ca766074a718276b9ee7527e2663ec7f5927d7ecd4777928d7f86145cb8f91919f6e75a883fd56b885ab9e8" },
                { "bs", "eff9602e3c03cd4f8b4ff6b080feb684288d7e130dfd7948f40c85bdc838db20441b2394dc1fa6fa9d94920fb25bb4e5ad0772edf6e2d8eae749072fb196faa5" },
                { "ca", "0072f77670ff35fa33643a9f66c742692ab8fc5b159911795a0b5e6cc28657af924efd82fe29283e937e3931d75f883fd8d34af59a855c67d9ad25585c7e43b6" },
                { "cak", "b47546c9f8e010db3025c2433ea97d290e48d5004f6cb74741f0048bfcd1ab49f525f5aa571c0ad53c72086c1864f0f30ae9acae7a05b3fcea8aa8bb74406a0b" },
                { "cs", "721c93df9dfb3683aa40930e92da4cd0925210ff6d67dd431c5582e9454a9760c4f59e9bb51db2f528573b051af0e735291440b63082559203eff1c8a6e418d0" },
                { "cy", "15512265798780163cd5c8cc02c77f772145aa1ae28e0a4c7db40141ef42b72527d9235815108a0734cfcb29b49e0d9e09b33864cde186c9aa44bb31834ebc97" },
                { "da", "30baa42c8bd6071e8d093b4713931b26fda8d4f4cdfee4a0438d27181cb3530eebe0c4b2a715be209b8e73da18656e13b155a7be29e0f558affbfb4a44aa6620" },
                { "de", "e6ea384ed5aaed75fbf4abb193be35e520bfe7a47d22814c98d058af10a5ac8fd3d94b4172238eebe4afd7b9d1338242dc51873658f02fda5345fe674665de5c" },
                { "dsb", "384c53afb9e681fb3c59cb905459d82b2f2dcbc93ae7393972eee5707a7f3a4b7050c3c54518532a462708a1babfcde7bc69fcb8399fd1334ea53f5370bf01ec" },
                { "el", "4d4ce5739b148b151b90fbd1c3f7eece2be2946eb1d2087da36db599876fdcfc7dfad9e96cd338ad18ce872aa6a124fd6ba896c6ddc887b63b221847dabb1d20" },
                { "en-CA", "85d8943cab635220d3c8eb77dc4c7e660b7f8e22eda3bdbccdf9df9e05a28d5710565be35261764a735765f8f2bf1e7e1a089b6ab7a72b756a5e74afc58caefe" },
                { "en-GB", "bcfee6615db9d86181f0e9a520167ddeb763956f0065a1f8e68f6fd95eff88a675ce7404fa8e72230d35e74f0ce484693348229384a007b011bc35a03cfad910" },
                { "en-US", "4b5e2f40cb9fc9962b008c3ef4276db0a8fd2898431add48fbc0664b136dd32cabf8c258a18024e9e32a2df4be6506a2bb1db0e8dc9e24c1aae382d941443363" },
                { "eo", "c02264c0702f26a189d8ce4bffdb55b9ec8bc2312c28cda8b366e22e48edb0ae5c4ba7f93fed2195fa69c4c3b3de57e4f47a2ab9b7ce32f2c2c360a0941c7153" },
                { "es-AR", "d90211bd419164492e4aa163813caf0fb285084148c031b354ea149cc0ed0f8bcf2a3afedf2ff7bbbb41e091753079ab58d4aa1fbedded4cd5060be0eead445e" },
                { "es-CL", "2f2df05c7cb37d974878d01efaa865766d99849107c5d92ee4e8c1e633c3e79cc0ccd33b9dc8daa1f56b20f3ba9f8b4afb8c0166fd1f454437c5b423281af1f9" },
                { "es-ES", "4eaed34f9b298479e3d184191b2ac510ae0077c3d76aacf87c89f4d4880c09623e54e5d569fe5c2cd46214dc23e81105878a5723d9a0bb4da10a5492f3125fc1" },
                { "es-MX", "780ef8b62084bceb3ec00bde9aeb7ad2772719790768899aeddfaf354b2f4af7e4d56d18c69ebbac017b6414316f77b077d0d5f50e61de4f910ccb66aa0a40ae" },
                { "et", "8fdfc183abacbe8fb4cb27df4ddf3bcbe10b8edd27f093d7b04f9a29de0b74ac367a05f6d5d1d7fe3400cd3523ffe8a4d9746a051d4cc25976a9fe4ed689b70c" },
                { "eu", "561e7d66694481a8f426b5bf0d3333e436cd87f8329d7e4fdb6499f6fb15e419bbb2049aba3bc2e8f287c32d9242b2919d9e436df41a62cec77302271fcef651" },
                { "fa", "130246c8ffbbd712307213728a59f83b02b727a01997ff9b877cdf570b0d0ad2ba6a8d1a4dd1a4eb2a10d89e424c9c6d8594ed76e946f61f3e8639b9093c2ea6" },
                { "ff", "903bf1b29d22e0d63c9184bdacb02c7d08d6f55c802a7a118a470ff648492f14bed3c0a9d80ba99d703745fffebbae9249cf7baf8cdfcc6875b05256596e2d11" },
                { "fi", "d56c5307795ea8c5208514f06bcac417f0b633e2ec15f14e27472ebab027ed682565a196d40b67915cb29411e127eb1db922def5757be4182cca04cbbd692ca2" },
                { "fr", "47590b0ba4dbebe7bfbc19f74996da677b9ece901012fbaadc8c124e575c4d74db096c9546cdc0b3530436ebb002eb9b3c8e5d6651af6ae2e1cfb1cd5423fa8c" },
                { "fur", "3a905c945fbddb1d40de6d26b291f0d3bafa7e6599152826419bb48f85fc3099c80d4210c1163cb235d3afa0ab575a56f7bea48bb077bac686a752a7850f580e" },
                { "fy-NL", "be3f15b81ac3a3c16831d0b7a874cb3f30665e320408461c96b120e1cc05b779fd6a7e36dd16880397aea2ed2ac59debca705f494965b5df15e61464a4f26043" },
                { "ga-IE", "d3136aa6cfe5fad583b07de5b47b08439715d4eb00e7cf90886dc67479a13bc3c8c7afbf03c207c3d10231f411ae4ddcb77841cf878ade686e9d27e805ee6925" },
                { "gd", "fbbd61cbb37b5c8bebb3eb805c1033c074545cdb0f24b6aabdd26f4bcfc01ba3caca102bd1e7c877c20fc23d040c71f0a2de811b5a0a66378fe151a56c789275" },
                { "gl", "f7b7fc90d25a095a06ba1ea9f5948f34a3997a13850ae3f12cba413b01157f1c65d1ffba010fd1f74a7aa23a07c5f4ddb516bf391cac99d999f02451a3855435" },
                { "gn", "9505597a5d0cd6dfa11862fd04c3bd59f7ddf7bb33cdbed39b1243629b7e5ecc44a8e0d5f3fc8383a76aecc3601beb2e35db9f498735273df82f1fae24cd6871" },
                { "gu-IN", "3b0f9053d8410f16fcdc50d4798ad3e551b5a344e60014f33c2339d05a74c666a3b4df5fbdda2c1bf3baa8d9baf248b0049c249ade3a0c0feb49ae261d0f8689" },
                { "he", "1414a798fcc91e79ee63ccc7f926de3f614ea2a3b09d5cd437f51abcde627b1e6e12daf9b7fb342c94901cc5aec5ed34634c2c9af3e169c7e737de0b628f8509" },
                { "hi-IN", "07311520faf047a836c1c59812f02c858e13a3cf364211de83b1e8dcf7e0cc7a6cfa31b1b73a4814d954f037598f93a0270bacd72651813f945d26f138055003" },
                { "hr", "04b1f21a5f5b48ad38504fcffa94d7d91624fa64f05fe3d0fe598851291c2d4be0a56b88ee650c553271b6fc94f3a9c37032ba4aaeb6dfd715b6e96d17c4416b" },
                { "hsb", "de852338105704b585178e561c4431408e4c733c80cfc5923d240a0db5546c05a66147b842cd127892bf4f30bf6622142e6a864dfb6957e1beaddfc95d0df068" },
                { "hu", "1e197258f723971d0c9ffef9199dd440e0f13ccff328c704bda837bede47889a3f35dc91908537d4e62fc2d79e9077be2c98e0093f3358b99909e1eb6a0ff28c" },
                { "hy-AM", "3a65b3c491355f799407cda80562e56851658a1015bb2aaf8360a2ba33abd30c7616e664aec621584d9a12b484b00583d15b4c45a950a3c913ca75747e1538dd" },
                { "ia", "fcbe51a44a4537c264ad469f0262436253330af37c8291db179a502bf4dce45237b32c6009f4abab5ae2671c111e3805b56585ae42d28b549cd4bdc0ed7cbf40" },
                { "id", "9b3009cc27694a8a4710d7ebc8744ca271503339f2621fc0beaa7c16e4ba36246ba1f7561b4eb3700085559c3eb893f89cbcdc1efde3369a33e5703137e5cea2" },
                { "is", "02c2d3de13aac751fcbfe78260b1fe7e618c0aa27bc296831714bfc572de1201d771bea1af6a09b73e449a392bd9180d5de86d2dfc704367e451df24d36c740e" },
                { "it", "8d949fc699196660152ed73e88eb2e3f8382dc7cc44417242595d32b6a965ea5ec12bd25a6f1a3f1c7c5b4de46e8794eb9960b0d79521e6b23c929b90656a11b" },
                { "ja", "88657127be144a08b5dd4187d88628b45636dc29651c191773fdf911a4f3914bc0304640782a53b3627567d1feaabc390dac96af466b40dd4c97442a7e689593" },
                { "ka", "790a944abe2354998eb249a79da6abda180cbdb380d91caf225cc7ddae7c4afd91f0444df2510e7eabc5ffa1e9948f097fcc41244b6565a76ecf0c12a6336890" },
                { "kab", "81a3d0cbf258bcee74718153dbe585a59feb82989d8fff1331a753dc349e9f39404cf57da7a158b92b260e6fcdab8029e92f30b3cd09d958747d936cc98fac11" },
                { "kk", "c0983eaa626df6af5dad007c8a290661bc421d6297d3fdb7a5e3f678becbdd12a38d1e58669fb86268014de8130bd486b1f4f516bed52e8c382824141f9ec633" },
                { "km", "54599a4451414edadd4b29946dbbdce7c773b1ddd773d3de78d7c9f7dc71a485c3462f56c60bdb7d578224a5f4bfb4fd45d4bedf4258a3b67184a617e4253a45" },
                { "kn", "94a1e3a8e6b1ac1b11ef15ce0a107e5d3dcef4561936cf348dfea4e02772b6e6736280a49ae39e97f37b640f5cdb894e1bb9dfa034b511af706a957f11319042" },
                { "ko", "9df5934ad4fba34c90ad0d6281704f5c58bd82c5bc9d7de3f4f545fbc921569d8ee73cfb6e1a5efcd42b0c06f2537e8bfb89201a41aea49575df921680a1a2ff" },
                { "lij", "ae59c1b3f74f5416295a7caf0267b83c19a901aa342e126217ee95e0dd016823e38a8becfe22b69116f682c4b0f8a918687a6d2ebb8a47264c2b6bf114c7db4e" },
                { "lt", "d57bba01011d853da66f89776aac2bb697010cabbe6d42d7bc8c0de74d7d0d2f5fd6d22b33f9fa99cc54b170d77b11706c515748ff6620b3d7c9550a715ca7fb" },
                { "lv", "c252c39d242fcc211dc4622f71f33eb78fb18971c56dfb022c250e0a588bac2b56f8f7655e8e2b8d76e350eff78a9a7c1c000d8a05bfd92584421a989d85a9e4" },
                { "mk", "5c547a74b7b45aeeadb7ac29634f313ee3456d5d4d45f05890dc759397eafa82e45cebee7c6f20033b6bd874af172c969a59a1a22b60fd6af2b3880f7b732790" },
                { "mr", "005421096aa75069ded7c55e438ba0fb133e8592a002414cc496d3f3d7bce312d1921c59d00a575e878e44138a32e2c23d79d0ca561b8df6c3b1be32c81dd1d4" },
                { "ms", "1ed8705f899ea7cec7232fae95576ec0849caaf5b93d3b70ecb3dc05457c5b2edbab016e9eb0378b20bc1de062bd667e54730f6be7accd816c7e96a5665bddc2" },
                { "my", "25ea91f317747f4ab4d0ef0178b50f0265134b7edf24a6fc6ebc87f4677874ec3c7cf95ac2e6b2e9bb85a736130bd4d5f5bc7e225fcc07f7c5dce95a701a0404" },
                { "nb-NO", "16be23bc7977f44ed00213fa5d6c9333ccf7aacb662086531d49a697c78f696a6d03d352f1984097a2bcd4e7129d972f867e04be41a52620bb9eaaeea8e22dde" },
                { "ne-NP", "06d215a004bc529dc69fcc2d4483a85421acd410e4b4a6f4ed9b06bf1fb2c7963532fa0833d17f3ae83cd86d1f2615fe286a4fcc92edc3235f1446f36f9ab4a7" },
                { "nl", "840065735d31fd4a4d1ada2e97282473be28a25af9fc945168944b633dd0f4ed36512caa5760a6ae9b2df1d6535b9aa0fa6760ab0b32dd118c31393f9cec7d75" },
                { "nn-NO", "0f344cd3c4855843f1e50887643bde985e2132e0dc1a477522222af3099c3a94788d514e856158e96f5ab331441504f572cddecbb5a8d1a69a37ab446fd5d00d" },
                { "oc", "f0b08a7c4f1220dbf14dee709eaeba5162dfcc3cd2f48242af1e7545e7717fb3e191208a930a9ba54dfa969e7d16a309feab8806f76401fc5bb05c1054880d3f" },
                { "pa-IN", "94655100fe46cdf0c5a2eedff71026ee2ca6b35c95f3d71566d642a60c6f8775a29acf1433f7d16e3b5ca6c13587395866947563374167dffd0bf004cb41aaf3" },
                { "pl", "64811bf183d7eea35016cc7486ab6198371ae52f8c1f35f2511c19fdb95463a9cd3b62ef7beab00cceac77c44e15e7e8df501507ffc4c54b9425d8a9e3f2f075" },
                { "pt-BR", "1713021c7697d86385d0ba5fb420392d256d5d9530e5e72ea4ca6d957d321aaa575190d7206ddbcf8fc0e95f4b86d249ff7229874c5a46240ede53d00ee70269" },
                { "pt-PT", "d4d5eb9fa2d5879cf4ca01218f1f2ecc1839e90a4bd38da59ac07e73328ca0f0d942457dc6b3f895cb460989635b0b01b8895ff5a5a74d5130b2f546e5fef69c" },
                { "rm", "68311ba46c97f975f6fb31c2b90e0f8479dc1ae89a50474357b8d1dd6c7e9357f1ed1f197e02720814f3b411654c0d8da384ce3b9bff06f365f897a74f2a8d37" },
                { "ro", "766bdc21ccd2b2e5b85c7cf7ed0d6935aa1d674c04d69cc0ffe095ce58cbfe7833fc9f930f6781b990d970cb30c7b28cc035835147ff7a42192ea4709ed3cec6" },
                { "ru", "9332042b284bbd1433915fe61265f693c9ab59a17275c269b980d631c3006499def49e82cbb1761ac8d49f98a51726726301d202d3b58be8882a019570cb60ec" },
                { "sc", "1de4fd1b1618f7280da3360faa5ec38b9b1dae28d199ba039793d91609a7608abf710b66942f9e0934f8496f9922915a4a6447c7121fb826a30f9866643853fe" },
                { "sco", "888d8958eb4f57f8ea2124670aacfcaa5f1e112b79f7b25089315f9b328c24528dcbba8c48b77c5600f68216b978794123037bbf6e4a55b85250abc477b9ebfa" },
                { "si", "33cee385ab7cad7a8b4c297e35c3c0be7745b200121ea634f4cd2ad57134e9159a7a3345b60789e7d826d34ee9b71410b2250df5b1f4463936072a3c4798e0e5" },
                { "sk", "20699677cb30798c86cb6482b6bb659f28ed745a122967025138ca5e664a11578ea1fb42370a98176b423ceca170d4b4d84bd5c4a51e14029c8cf2b95fe5ac38" },
                { "sl", "26523484dbe65f85989d7f81648e58b9f381f82b82054006c74b2b56679af4580ccef9728431b90561377ec8e47cfaac3eb15136deace7bd6f45b609735913ea" },
                { "son", "434592f592485be9b91cc29d5b771fe1ff49aa47fec80037e89f125ff7ea360ebd3f58bf3092315aacde3eb85591e61e73fa1fb8d9928c77d635f05015fdf7d0" },
                { "sq", "b762a45c0a0528483415e8ddd0fd4faf6642832c05b1bf86c8eef580a24393967d2cfba2a4cc97a89534c85fac980fe537d01dda51b483df9bec56fd81fd0c2f" },
                { "sr", "e2af01675c945e0dc2a44bb63081860383d7efa52150ef314a71597641e17779a4c666324d673bff10adacd47991ca3f086cf656e11846d588e403f0cfa515eb" },
                { "sv-SE", "fef06640da10f0a9b4a442277c80834eaa1a2d5062a6705f7c409dd51cf1eed10ac27e3e47b768146bd4085730ade06474c94e70da4da983a15af11c97af8a5a" },
                { "szl", "d7b3b4624dd218685ae3fbad0ca24f06d5181d744e993e4ceac3909a7748e25195e47c2bc736ee2be4bd0a5f7c8c1a8d5237724d5883f6a3b9d8cdf14fbbca63" },
                { "ta", "990d27863e41dc01296b5dda8af2057961f9390b48f4d79465ee1f0ff42c3daef334650ef3b3c34c5f2f8d5d8319e9bec5177b1f740814131c5ab6e4fdbbcae0" },
                { "te", "97c0fdb557b1ca91f1bc9b966eac3e79b99a15d9ad79bfd93fd5040f8d9fd760cd34f0b302df91210c64c38f5b45c25e6e498c6cd0dd4ee4fb8dd947643a981d" },
                { "tg", "4c07410da03196ab6753f181e6f3a2a9be7f27127dc559d0521cdd9891aa4ad1ddb21bdae4ba908a2d33f31d2f60628347871d86bb1071953c81dfead7b09cf8" },
                { "th", "4876b99fdd7fdc82d32a12475bc9423b59c24b7b86eeb7023c0763b3880ef178404de04d2d7eb26e5cedb02bb3f246a52c4d8ce85ecaddcfad54cea1e3d04fc9" },
                { "tl", "8ac4fb3124060cc1d064414feb062689d2d226de2def6da0c02a054dce3dda1e851d7bc364ff9a35ef61cd1f0f3fcb77fe78987a06d74f0091df04aa932bca1d" },
                { "tr", "201e1acdaee8f0cc9db438276fdf2d0446c49ff4c0541715ac15d825bb49990d0f2a9a49ce01a166d3d88d6c308a72431920a95bc4bf8137b98c109f1931705d" },
                { "trs", "93fce7e120fabbc52ee99646dfc51a8e4c12967e6c0cca1bea4553a502dadf5d7378fc11988db90634a81c1ece88d69167cb83c8983442dd7e8305a8c7feec0c" },
                { "uk", "1ee7d7251d415fa13a511e579246da3042d9ad1c9f9be8e461d108724a38f7b2db146ada77c135346991143fc08aadf90de6ab0ca5a7d201d363737d49717a61" },
                { "ur", "06350fbdac8e932f6fda9d9c7f3d6c0adcedcd8276cd79484486be1bc35fcbb94d795a7f61cbd2c8e731d945c2fe90e5566f645dd088a37aba358a9fe0b1c11c" },
                { "uz", "32c43d95685e2547fcc7d0b985682da299d2321008edd516360ba9dbf9eea3f9d0de6c523ac5047a7728240746b47e4dffeb1cb664db6a23f1907628b5bb9fc1" },
                { "vi", "c6bde6563ee4463342eb1746aa46699fb9c83e5928698925e86242743f9fc13d5e7c7e3bf5c47b23e488f8c206a9b94309482b1a197c183ff15f604756e03c6a" },
                { "xh", "897d43d08bbf99eb4d56d4092f9d1a0caaf10a7e4c703a1628cc433398e3e811a4b1e920ade0f5c50499dc91435e3a5c6f5e1daf740b816f4609a80c337d5def" },
                { "zh-CN", "193d5ad2a5dcb1c0007b0269d8b73165c5c65b5b0869da8262ac7fb4fc1cb6efed4d60fa2294d4c4345bdfc48e04e51fb15383fe07fe776830837fbeac7a0e19" },
                { "zh-TW", "6bfabaf02079e7dc6bc1d202d4b9d337a7d4bea4f724324e90be3e6f5e8defa0af6678557b9662f61e87293b095683016b9013662231d43f2094919b4dad2658" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "5534b585e8cf9a7d2f932ffae3d25fac16b7aaad16fa5dd8f4f56215869222f451aea12dc1567f53893372b112dd92013a5d19a86d8f9a01bc30ea7f202ec5a1" },
                { "af", "46321fdc35b28042aa1df8174cf134ccc9fff5da8ce5459b778e623bf822fb4b54567bec53dac2b26c88d05f060c35423c491c9c3788e39eb76b067dc3b78283" },
                { "an", "638ccb87eba10f5c8f5a861e8efa4a9ed9e0c96cdca7ed13fef53ac53dc5c864c14cf5975cc6daf018949638a758dfd295fcc3ee582db51dfa6dd931c15b73a8" },
                { "ar", "cc715dc7af780dff6322cc4aec12a4c7b40d8fea1cb6110fa8dbabdba6d0bd10aaf275bfbc3097ec88c8523db0eae427c96a913f1bf9748bfe8c8f98163c447c" },
                { "ast", "b2649ed1d7835fe883bcfd89c2a46f1cdb98805bf0c69d764b187578dc4451bcc6cc132ac9c837e16f776bcd8d96fc36b1313b901d3535a4b54a3d93bc09ed0c" },
                { "az", "9106c6e7801cb6dae59535f5f18ac27bb30761d0e9ac9cde1c4c7afaacc59c8d19dd415cb6c4c242e49d74dd37b6748cf5b467cdbdb780966afa095e631571bf" },
                { "be", "7fff788dc43c653ffaf94223f108b59beacaafe4b038db57518e6a0ec09931266b10a2a6c543c5d36d2e0f3a0b34fd438a2ed8badf6ec179e8e00d3cce0cebff" },
                { "bg", "276de7131cab05b0a93a0b0c3610ae8832b271d6841a44db0a902df109c7340f3a06d94585a0a1bdea2fd1a86e42b303f3e2264ed4a9872a6e82f89066e8760c" },
                { "bn", "28d0f470403a11c78d238ba800fbbfa7f2f56ff7e539022e165ab9ce704653e45c32f7a72777a00ba35d7e08c72c9ec1bb7e341ae8ed7ee7bd02dedb24f20f9e" },
                { "br", "e2a91cd54d2545b07d7054465f672f84f33e5a57b1b578c916f768a5ce2cad2b5f2a2ae2187884de9c623cc76d81f3857d7067c05136a2b2d3e6e68040e5f50f" },
                { "bs", "511fe4ee6ec2795669a7d3606108ed0fce7ae039978742cf0bd7a81cd0693ca2546694ae1121a7fdab2d518f3dd014990e0cfcde9a2188f8f0e2c21a1de02251" },
                { "ca", "16ffaecc0512387fb5f91c4d2bd7e92102d086377ab55b5585b50b333f20ba5284d5456b1435c1792e7ee16d13cd8177fa447031844c78c1319fa6bec0677fcf" },
                { "cak", "dfe1d95fe6d70cdb3ae1b9e94d192360be413ded05ea5c0d9ff93b3b6b92bbceb22ea9e637b31f00f4352bd5fb3a7719c26bf581f42283ebb80ff2a52e30b0e1" },
                { "cs", "c9da0daadfd144d65048f10917ee43ab4d84507d466ad8f085a8c25731c1a28fba6817242a4bd99c3d32c70780d4c9a68d5d73cf8dc8b77d0890bd0a5bd88947" },
                { "cy", "3051eb245e7ee27c7d9bff94e8dd2bb7712b76a8a5829db3658fbecdee0617380ecc810e48dc8e5436a0102187ba0fbdea659ce4ab59c0916a62635986c1ce93" },
                { "da", "e69856a9ffa966d8bbdf13742c39d7161c4043ec5e0fdc80d900830fe3b13de559afc0e6d6f420039a092ff47c05bbcbcc179cbd100dbdbb774278f0e94e1498" },
                { "de", "8dd085d92a43929d1d876bdb6c94de7940e0facbe0dc0355b3831a4d35c6ee568722e507a49a0c1e4f0ef8f009fe9be2403c81ff5e727bf21ed200590a28ae40" },
                { "dsb", "8fc047b4c8d7cbe3b2210b52dccd833e324500e7f4573435d395fcdada1a9ac718b10e6b0735f64974788b5c3e73a25bb1bf1dd0fecf8fedf993e778e69166c8" },
                { "el", "49859cdd1b7c674943775de35131863d269ac5e83cb7a131f7d6ad6fbdaff69751f540931c17e581cf5959dcf5a0acf33ad89a6bc347912c8733ed3b22301753" },
                { "en-CA", "44eed4f6455b566fb4750cb8fd372681843bb83c97f8f9911ca097db9b3c0cb0811c4b08864c12e235ed04804f325e3d220da96e83f51cd1e815c5463da4b677" },
                { "en-GB", "f8354fd3496be979ebf339cd74165cc282af0a3e0e8d46ff8a10170f230a2c876e81e2918a9e5bb130cc0bd570e8051d583288657c7fd9fb21484cf3115963b0" },
                { "en-US", "d2d26d4e7fa7536265be19a69eb983089d2348310ca7bc777f1bba7760234c739f65b4b721a104c19910a9b79c6f0ef48adf98866c25065ceece21dd2d8de1a6" },
                { "eo", "2b6c8160d1be5e1ffae000aae3537958710563c07b1f48ee7c06308a0a62f99e4a166cda08b7e196ce7af4371a3cda6cc004c81af2f062293ae850bdfbc16a63" },
                { "es-AR", "ca8ec88a772f6a81b545e54cb4724e7eb2ee72548a3741697b05bf500deaa815540495f4124d0a10e7c741d96e349ae314cdaf0a1c2c3171aba3537f51e6c95b" },
                { "es-CL", "5957537d0bb912f169843d70ac56ece509f64411936b74b41d46aff1a1b648457f7ad28d0e302a0c6dbec6321c10dab586df0e7aad073593e849cab71cfe8c0b" },
                { "es-ES", "6004be89ef9b6affbbc290dd5f891efdd8a2258f38c317040405abdc435bf34e0d08d4ac7723936c9deee8c9c6f9aecb684ca004ff5a281307c18d6be35d5e2d" },
                { "es-MX", "38ab6e69a157ecf4f73babb830b41e957cbfcd47a98cb74a00e11dbe9eb7a54a5ca531b1906c8b33651d3be0cb1bb47edd8cbf7e44caf053057a4aab55195f41" },
                { "et", "3f2c010b9ef55a081409ced8cb151a8aa6504550caf4d0832f97a9ba85fa5a5fb7cafa423871bd5307ff4cade23fa21b45bdee56544e68e732e1c8cf28e04d17" },
                { "eu", "67c6d480d9eaf11d1bc1cbb8508228f91997fee116dd63397411e63fa37d066987f113df509d06b56386a31a41bbc9f45400deda98bf2c8943b33bcca3612bd8" },
                { "fa", "b871c0d9b2471d2143587ef456a8d09a96e462fd177865de3247fa916d9751e5c04eb7ac0bfe8228439aed81fa283d6545051bd8b28be3091a11ee5e99775e18" },
                { "ff", "260a8700f48237ee279f7ff820a642735c02bc359f950ffa694f12c9e945b22b2d6c5c5daf724f628c341f318d5bb8d441a4883f5ce4f3834c4a36c7dcfe85d5" },
                { "fi", "df34d700f2800030d6161a7a2ede0576df694aae2173d0174c641283ee0c90930d82e49063472bba66c2b143bde35944006845ad7d05455fbe1f4f8368d63fb4" },
                { "fr", "6f39129dfdda5fbbc3be8b551d7c36e4b2d8dc7a9f94d87e10ed5b4cae0f7c021fe446071886ed52075a9939fd74afa5ab91f7d388a9801238caa99ad94d7112" },
                { "fur", "a88d9600ba6cdefd2b91eb7d27c1b57a274aa95d2845fb6fffffd157f40fbe5fb3ee8a6b046d5a0df1c9922cd8a58f5b036322b8f17404f9c04145b6ae9de136" },
                { "fy-NL", "1a3af81555c98281190272eb824491909c375f8b1a871d6193b6c604f930022403aa41c32afa2ca05b9f95e24b66f345d311ff0fa3c92002362a5417aa2b3aad" },
                { "ga-IE", "8d32fb9a2c13ebb5f625b1cdc5a3816b24541b5eabf3e00a01cd2c62774ff97330788e830bc9577996bf45343cabee4f4cd6f683eb56cedd0c1fc8339a7547ef" },
                { "gd", "7735520870265b6455f1dd72824c0dc962bc94e256d4f1985ca70bcae5f19a62c9676f0deb80f1c202dc68739ddb351ea2e80a070e0cbfc1c007183a016cdac7" },
                { "gl", "496dc63fdc5cd660287de11312b85a5e07af5b06db810d3504fefa4ccee9c8fd7390293f566493db8356c1c53318ce3f9c5d41707706395556cca58eedb89f33" },
                { "gn", "fd785733fc8415fbef566586f322cff1f10c2c7224bd565c12b590705fed750be615385980524a3d0dc1484708b0a693079faedd62ca2ab9a7153b9ab41d8535" },
                { "gu-IN", "42137ad7011a78ffae3bd0134d60438cca8d87a88340b5d1e9049153c57438ed013316b40b89d2b7ececb221b9f9e7f3acc05ddf29c02be2bdf25ece89008666" },
                { "he", "e6a7e9445cd08f95be0d6c67989c6fddda28b63feaea40f2b0aeb68bd453c21ae82c62f79ccea44a28a42a99bd8ce2d554cbf1f3ac90ae918951b1fbe7f68093" },
                { "hi-IN", "999577d66c114b7ade1cee48536215e7fce3677acb362d03cc39f044262f6839b9fe0b03dd20cbe2c376c26201edcfa01ef569608a7903de4962a20bec0ade92" },
                { "hr", "e1bcb5499f7f85c528ac427e447a128d43f438da7f6205294fb7ad5766c9214244195a91726baa87f1a18ddf53d3405a932f1e3354729423b7db18690bbec9f8" },
                { "hsb", "d5f567fd048540135b1446d6c89c3d761ce2e374fcfec0c76aee5cfa47c139e35bece8d21919b2cc5f4136d5ef9a9f302e1bf919ce85df3409d7e993298adf7d" },
                { "hu", "70d501eb6560c41fd69becbe08e3eed5de0b6d9ddda14f0b10c01bf5c4cc6360424d69e7b8a66953f89a9cbc6a6e00bf747444ae1cdc8463c236b09f8f1ea9ee" },
                { "hy-AM", "ea37c404af47fc0f154aa2146dd7e62db9f5014d1d27e93119306692d44f920e09a5cc0030736ac06c76e09143bd04e46277ce4b5ad20fe9e377a344cb8d4713" },
                { "ia", "f088a92e51b910610b0b27469bdcdee3a5976206833280d65a0b9423b4c2544319c387632d72d18cfacbca126dbef07510076d54a1b53d7b8566dd927aaa09cc" },
                { "id", "1079cca0eb3b92391633ac35d7f389e9e15ba012a95e1329a7942d1895b18ee12f03ea537e980cfbb402338795da0ddf73756a2d91080898b2ffdb61212147ad" },
                { "is", "82027c4e8484c220210269abb7bffd860d66fb01733ee78842e5a10ef6dbcbe5badc1ae9fbbfb0c028e42ebbc89a5ad86801e71179d503757a3a7d93cbf43f3a" },
                { "it", "b23fc876fbf7e3f8f91e793d90b45e4c59d5c63a1be28a8614bd4f3167c26551c702f2922d3a16f76e90fef98b6f165aa2f1a8d1dd4fb3f91217c9a2c1c64814" },
                { "ja", "f8e037f3622fea352195a3cc736ac2879ec0644be7ce52393813e67231300ecf3d7d69f73977f85d20fc5d6e29e00ddedad10ace7e9d8aaa8dd643ff0211b6d8" },
                { "ka", "4a427d6bf4ce84208b395d66847d2c2a8f5e918ab4ec534d61b6013852f972cdb791cb7e243feaaa8a1dcef2c1390a3329f23d342da84c26f85e9472a389399e" },
                { "kab", "aca48ee6f5ba54ca9ede25e512c2796d71036b72c2b99de85971c48fe43c8aeafe0e4f34b7b48d78998cc9b10bfca5747f29addc73a7e15cc0ce489212cca3fb" },
                { "kk", "c63d0fd3b06e62edfb1b873682b967289e524f489dee6fddbad815bbaf3c1e71cad04baa8920379a4efcb0b1b0e1d24c407b6a241ba39c42b80fea1eca0e8592" },
                { "km", "21065394574be5209199da335fdb35fe93960171ba97eae4d53caa2050963bfbb2dc03f293e632fc3536a26965a36c8850153fb4ab07c83e47f393596c3049f5" },
                { "kn", "aea786e8f42852e191a9521bb66bbb51ed08ef803dec9a8efd97e7ff7737680f162f1ad2752439b45837e9e675e353aa98d820a816bcd3db4db7089783e74d8c" },
                { "ko", "18184c8f940560a9d2e35df03ca6312847a68ce3dfa750c2576baad80ca83b68f3c0f82b1c7c0ece28054ff3a6a5364b6dc3eec23dfce5903df1e8a4c3f5ce59" },
                { "lij", "1ebb05c3744eb10e83c41ec57e187dc4874714c687064a6a0688b0ba254b2fb4d1a188881c2d6f89eed34e1a78db02b1826a8d10cd08f513095e0593e1ec144f" },
                { "lt", "781a4d540b7e0ae891851596e6a526d259bda8745ef710f04d5382c56a0f60d5607a02e24ae5c9d7c5e9f9aeaf099dd548125abb0b5c99ebbb38ffbbaded5d7c" },
                { "lv", "2502e09cb45324068ae21b44b661c1ddea9192a2f2d049a8158d4fe57c532d458e9cf0faaf241bc3aec79225b794b9b5b570b50e256e7f5b66ca464114b7505f" },
                { "mk", "b332ec1dffa1337d36a454180bd873101249c2ebe82fe87210c0259282f2a5b5824defc6115490d0cf5a55003d09c10985c7f70d6d721ba1e133c30f9acab69d" },
                { "mr", "56ea7bdf101073540ad2e8aa15bd2f748070008fdc1f0bec1336c1ff6affab9ffc3d69923430a320b0171564ae3409c60ae8bf9086bc6ac2d16debc524d172e3" },
                { "ms", "7bd029665146f373fb12fea4641e45e0d4aaf07276e4784135f7426eae90001f1108ed9b8292198cc401637321b0d39580f4b580625f0c293b5a36c8dd80e46a" },
                { "my", "97864078fea263ef291d648655c7225fd1f99f3f7bfc19587970f79590ff6f904c23a1377165025e8eb133e3dee93021f487554d9d049a68972ddbde7aa8cae3" },
                { "nb-NO", "fc58498d5db42c08f96ff9d57a24a8f5463da5d7cc559ad289228b947b83612ce00826d9fb94dd0a3ed57d324a0cd3846532256dd62eb55e952839b46ded2bee" },
                { "ne-NP", "e31e235baf96155d761af6b897a0e6664bfa40cc5f37e80e4317f6a9fe265ab90d803a4f01fedae4cd0326c655e6c44ea06936292e7873f63ca859a29b44395b" },
                { "nl", "5a33595c9d6c381a774245ab7bcc99fcef73a368125e0ce0b22bdf9863c4cc1443bb6779f73b44a6963fe26a50dfb0d52ec9a829c0d5a8156dfa679ee9764907" },
                { "nn-NO", "48f99546c1d4d5c22a08ef2534ffd2e687bba98e6dd6666fc660563b244d3de6fd6c8e71313245fab3f30855b77b9c796de709d532af2df7b0a02f2a44455c17" },
                { "oc", "21bc1d02842f997a9a1f185e55d40ba999e54ad15fae0072814e945db832e6250d3cc36cedb5c86bdf1c62da832e378d4df10405bf60e2dd470322566e0ac32a" },
                { "pa-IN", "56cf3537e084ac11281d7f2eeca6af06164956486d6d9b6694fabec7f0954e679d977d68f1b5e712d386e1d58a93a78326aca18ed9ba30746740be61deb3faec" },
                { "pl", "807dc7ab2c65cf3864d28ff7bf2e328cb4b479a4de8733253a6b3b6163c9dedab0617828799f3592e1c8fd2c3821aa65bf564f35160a7f49c73ef46fcad0c829" },
                { "pt-BR", "b0625b4831677f9e99760c011edc426cbdb1d18993380214a860c75ab2ab0a274285106dc493bc52d25e16f2aefe30d76933a0581bbb9a94afc7af5ec508f1da" },
                { "pt-PT", "5de8dd599502f2ca70425735c517c3bf01153dd540d13a59c1f7e79a9fb97420f339c55de8d136b1bc87730e5bc8ad2459963e51095e4c7fe178ac3cf4fd2396" },
                { "rm", "b763e062e6e010f129a954b4d26ab4769bd49e0fc96f4247460eb2cf483b22e8d90954fcad3e8422fcd2258bb597d4a769574c16356a870303acc2e9991cf190" },
                { "ro", "2937843dee0e5420e671c58fa24243c5d1e3d66c6dadf69ae0c4f1cb6a42fca875896688ba1319b92fcd5c55039aabd7e4ab8867ff6fd7ccb38534ac1c383342" },
                { "ru", "de87f9693f303b9f054e61243ca17efbfd6bbc3e5b939cd2afda22dafed68b5a4511d02e3888dd96e61ae4807ffa33827f52cfd3f66fba0736e97a761525135d" },
                { "sc", "92774e5818bd746f9df80a4afc9d191356f47de76de37e9087776932b430a344cb39bcda3b5210f15e824ba6207d5696a9efea4b5f050fa6ba0d7d64373cfed5" },
                { "sco", "d5fd0b9f2c5c52f53b8082d00396bb0746d42542d00dc333a676be48e3689cfc71f5539eeff06283b2027e382c3b1886456bb9974440b110b3da2bb4f1693767" },
                { "si", "caac9bd9a94699a3c62ea3eba4a5dae8c9a137de2396e416e045e9f883cebb1faaad797c7fdec538c454a806e9635be56f5c1d3ccda80be2b0b09027d6427235" },
                { "sk", "f8cbe8ec38c47e237b8e227f3756ea7566fb8abf7917deb18ee861ca2eaa3deb174d900c8cdc2e20fcad23bc956bf4aed70e41e21828d42f999456b82e221509" },
                { "sl", "a00f81013586c79782d9813947816651e28236790e44b0f3343c82d66ca5a4f2ead0e151ee1b2d78ebc02cd317c7b7aba6fe8ee4047ee4ca37b26aba5924033d" },
                { "son", "257ba371590acf7d7d42dad8be688a31bc9a5c4aa1967bf74ac9925e3d6fd8f1db87bd08befacf388784785396311e18f3ed4d863ecbf7e7f0d80bdbcb2f0bac" },
                { "sq", "1a0e6ff84427bd206d2c08b442931054721333889ff3668c4740c1bca29e7d7e09ef027e4a2a69de12f2bc19f30bbc079a6b2d72c358621ce3d09dd96c6558a1" },
                { "sr", "eb4f7d5ff1fa0d9b869427026f95b27a75825d3a1216b4679290d5de41be2d948d721ecfa7f45d174c3d6770bf751f1fa9cde83844b21583d4c1dccc347c4e0e" },
                { "sv-SE", "a64cf701302d6d8514d82b67b854192be25da4d8eb7fc403dfd827ec86806a45f9e7d5f5476deb20ec89df30c932455d1e1325f37d8f791c639d5367b21e4b01" },
                { "szl", "c71a183997d1e12c8d101d0bacc29a9de6767f65561cc3614355a1ffc674cf815ef60f91c878f3e6ebaa8255792129daa1a91aff9f960156e5fa670d02b82f52" },
                { "ta", "07eb298ce5c65dbee8fc1e66dc81c65ce643d4e89ba865a64f3c01042052a21c21ff0dc281303e3072b46b02d2d5095694bfd86b6cc77903696aee6d112d8e51" },
                { "te", "cd3d2d8899a08ccd427b9efa6ed8c34d2b947a70eeb734c301eb169062007684ee129a5667c34c610485c63833c7c6672f88b23e18790e5ef9452d0e9ddb2220" },
                { "tg", "f9205a3e68da4564701f8c0618df48f4116deb65cef25db3973a0158f1d857b71fdc6c0dddc3449291b20a297d45e610cff2e977bd4fa69b0cf7bec58dd3276f" },
                { "th", "6de3d961dc7f2378c897ff91401ab11cb2f7047569e26a1cc7ae72549a7516bcaa960971ee36c496ebc6d16f0eb7045498a8d5352acdcc11c5d6013c7c122d22" },
                { "tl", "4f534031b1e30360b4b2d612d109f3bf0e4b1db9e373e1932e3c247f86de8b7f6674175e121f571ccd67fa1890c6166b357e081d237213c2f81d0d48881bf842" },
                { "tr", "bcea283e05f09b921d338e5746b28175bdac0acfeb7a05e4f9eb586d35c34e22c9d07799b9d952e8dfe980440e3908ec02fb2cc58702ed435b75792fc11dc5ed" },
                { "trs", "dea98e1202e34d6ec1b7f772de2fa973f5259e01a65ce81ae6192571cadd8025d092f2f238d4963a1f3b516e0c848f4a74cf715b0b14fabeb2a2fed3ec87db6c" },
                { "uk", "18fa472b2a42355b68f6e7512400f1c0c1ea454783c37388be168c9ae6ed5c835902fb419470a22a71743bc93130ddc6b7999b7f1fd76dd3dea60c488c97e87a" },
                { "ur", "083e6d24f84df9a19c11a54927cc8f41319865a4b27cd8521015192f686a314247dd7edc3d14a0e0331b48078441bd69cac1e1773d2a9311b466439771b43048" },
                { "uz", "7b9029665cf68af3833134c1b9851e45adcee12ea451b31c1194475789e956939d238e3e43e82e2beaa0c005780c2fcaed8a19950791bf88b9ba9c6f1c730f3c" },
                { "vi", "14e987f3283f44120f0a901184cc6bd91fd29347f9af70c7b8bb8a8817f840efed102e51fd69cd5bc82e151d9e6d220f3e1a35d285dd4429f3f0aab39da2096b" },
                { "xh", "54fb4dea9e84b6c8e60f0dd1814fe0deaa1b282d74c9eca6f102587256a01110b38aee807e2c397cde998e043490349c3a88da843cb631ffaca8ff512d67b685" },
                { "zh-CN", "cb8d7deb8852990fc614e32db8a2c75de3e7cba7e7da9e791890b3913cea19be984a47f7721e9fee329c95f192452a9f963a20813c36a246f322b0f4f16de010" },
                { "zh-TW", "b2f95966bbc199559922d170a76e37295997d3aeb75c988ecc4979d590905179a8bd77932d1cdb9bb23467c76875f1a9be94867d51738fd0f080b8814e384a55" }
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
