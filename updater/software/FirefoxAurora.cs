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
        private const string currentVersion = "112.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b2/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "453a9a33017a1f465a733f1ba80d2e76895dc29234e4ceea22a736267c0e29d43476213fec3d2575d6ceba093b6f235c7df87e719483525e94c42249c7a6b22f" },
                { "af", "8b06031dddcd6e8357e44d48921a2179671e184efabb730ddd87dc24ab444b1e64225ea8b8255295ea784c72b4e8b44295903d07cee3ea834d11e5a7cf9cff49" },
                { "an", "c193d18b70fae5ffab367be26258bf5517f8e51f8ca61ac45b7be3e6b7f3b0a5251c16c81d3253a400367d74346ca5e2152a91172a644f5f5e83684fef104a0e" },
                { "ar", "6aff25e777cc676d7c1d7e0309340873a2251132656c9ddd463be5db1172c3abf0ccaa8227f2211259703ff20b77a3a0e3934ab814e38e312909f202323fe520" },
                { "ast", "ba5df15b09c177ae85f9665bc84d2b683fa5920b658e5288575e6cd2dea134b543e1175f81b7977695951d86cdfa68ed8741e37544d9978c6de3d0385c47031a" },
                { "az", "8871e8ec220b75443b3dac6f93c5c94229a532445e3c2fc8fa38a2a5baf5023bdfb37943b451c70da8c6d2c7107a2a2aba2be693bbbf6af8fc0f5ffb1cc9d8c7" },
                { "be", "0f1ac2dfef1d2cac05bf161b204a473e027f7db345c69c310c2516337c82355bf8f030931e38aff1bd4c8553b16ebb05cd78cbcd9d1ee00c1f464aa6b233e7ae" },
                { "bg", "e4dffb028c0cdf47f67fe874f47b4eb394068db7babad20f075287cac3d2704f33856b7a11627b8cda8964dbc554734a79bc5209485ded2ac1125a67c1e76749" },
                { "bn", "00a701feb7125c847fa303e796e75e3f17c1df50fd6cfdd63ead5a04044a1a0d9103d4d1676f09952d80738c85e66b21dfd55e02f7dadeb66403af1286473d08" },
                { "br", "ae74f4357a1ef05c1e4da9c96aeabe76b512b37653b2d4dbfc1088eadadf46caa7921e597d0b036e90bbff25c7e86b42e0cbcbe25f5b081630b056c63797ab1f" },
                { "bs", "dc01447c3f44eb8c7e7a6832325ae25f8113d93add974e7fb15d9f5e36050e75147a28809cc685d67748e469667bfd32d195a447c1b1220df08badb7916ed9f6" },
                { "ca", "1130cf9c5f046f4f8d5c69174b4ed8b19cc9a9f7225943e15759cdd6d435e684030853a794f67db9cb5c34b23cdcb1b98ffcb36cdaaaac26f6ca3c682fbf5134" },
                { "cak", "40efd9611596231c925aca9571c8603847782ca7d819bd747c8ffc39137516bbb0c127ca6fd206a833cd5fe2f1f04874bde7679c338479d4d373daf6881e0955" },
                { "cs", "2e52fd9c1870db26b93272819761af11f64562f36c202c08affd6527ac0e3710fc95f9cd45558b7d2cd2e61f4db94e39700763c17a0d618c02f9abd946c9bc2b" },
                { "cy", "f0cdeaebe74ed17d929663f94224762f7dec5fc5d4a23d0f3d72c1c93d16642528f4243340fb079c1fa976bacff45febc0a5addd7a01d6226c4a553f77ffab80" },
                { "da", "81aa706104b1568e05b5469873373a29a1baece9a9d8c272c4c123bd6a99742252dc99fb11e2dbe2a60314d1009ea60874c6635c6fa1ad77a5854a275f48d909" },
                { "de", "24de06847edcfc17adae1d0749534d7f9314910c21027617f8d0d2be6e9773571ca6039f5c5307e0ec7ecba302f7974d272a80b8864a71d779fde59c77cfbca2" },
                { "dsb", "d49ec60d7f9741f65c4d4fcb9ea627fb87444f6797cbc9045e5a8ed54c0ec9e5d2901605bc29e9c7001375b3d8c98fed21388be2340c75bbcd6f6642fd3e51d5" },
                { "el", "8fde36d5fe795d4fa81056a449b5ca591fd2cf8b04aa3da936388c836d4a8c113e891c9996ef4844e4a81f16b43f1970f99550bbedfe9629c0c2f8c30d309321" },
                { "en-CA", "72fbc8e3531ae2dd22e42acf1f2cf2177bf34a6e83d32cbf8e4c48c23c44889f1377cf5b05a7e7e471df0cd3c6d88c243d30a6c7342482702cc6de850cc58f27" },
                { "en-GB", "eae58890120463b538a6c79a1fbc25fb18b7030d83b831d0eb4410095db1baa2777739e799e9dbba6eedb1ab269934ed0c5b35e40051b764b20c7c10dd14227b" },
                { "en-US", "6b42a1ba7c4240ca89e8f9e16d3c221b113373fa17c1953fa6f9b9963495a13ae768c12de686f4f824677779899b3d399e376bb37439f62e5efe9c2b74fa9a9f" },
                { "eo", "64fc3f4f8e7453ab6d75920e04602af32b355ea82e67f64c7084e53dffcf2faf3e6a24176d914750f3e6021e151f2d6ca51ce91e7a3dde66f83bbb9090762be2" },
                { "es-AR", "c69cf3ff89a9355c08a053cdc3cef0c7b45605036746d42476ef9d45a65d583015694cf57574e249e0e42ce7dd677f16377843e544b69c91a19aafece78003cf" },
                { "es-CL", "eefeac16b39925e1b418bc764e3d7239b7070ac0c180b508ef29abaa9d4a448198788030740de657f4acb2793601ef1ef4a7ce3e8b6e019b347226609c222482" },
                { "es-ES", "213a7c35e8898a2f2c76719a03346e8246884187f4280d3a5e181430873dc5493f1bb2e6bad557b032ec08c62c0d161d2f4d4f08ce5877493e5e74c27f0365a8" },
                { "es-MX", "830c4dfe2edbc726d1edaad2a9f601988c2054b6e6835690532f4040801feae60b1b5d67b2b0caf47ffd4735270719fd425878f35a6c91c762c7eeba3bc7cfd2" },
                { "et", "8bce1c1582433970baa4ea85c7af972b27b95926bbd5b5970c990d7dc120846ee2cc2948a5b20748e98832481c321788aac14126dc2475feb2e023de58431798" },
                { "eu", "8f3b03fa2e2b3296045c4a4853c53871c2c2dbfbad9767a94028768bc939f448579a48f9125de192a54b2e4812206834f055b09f28314da265826cccaa91fce3" },
                { "fa", "5bc7993c07bab16b21bdde95c29a78f57c00c9d09ea7b4a1973d15a68ae8fe61385384dc2e376a87f4b9a6f02effc57855896cfa0111f5d5b0954d3098510702" },
                { "ff", "d6e9c244c67bc482f1bccd483ce20d1b9147b59cbeaa1442019b365e677e0a19562d078fe5d106699b09a6a7f91b77642a6a7bf1d1d849866f1dec1b0c6f5ee6" },
                { "fi", "5ea4a9ed7d8a06ece2c3ae0b6b7847878fe4222df063390ddc1e6d79a48c4bfc571869054ea4fb2a3b41d43e947df6820af60a2c5ecb5ef9ad2337530340cc8c" },
                { "fr", "aa4466f5d6b438d49170d296efe2ed50b2b9a6791e1824100f168c77c5d453ab448b6b5148b7449bdb690e577b6f9c8337b63e77eabd265fc4ee700a3ce6805f" },
                { "fur", "4fcf1f42c8c97c14158bf2e4121d8b16bdc25de2c8eb7dac9459f4bb3129db88e2c66fa199358095413000e499a9ecce271a1675cb0c7d9fb6dfc166044d85be" },
                { "fy-NL", "35c17ab55b6b45f2026cb8b6d9bd90e926432f31e4e542f1668170787bda0059970352db671e76aeaee4d58304e878db63a3e96b09db4a4cb2eb7f41dd3bbe01" },
                { "ga-IE", "36cad435cb97a6b543845911095a13a038fdb0358a2af498eae03ef04152437b61669d6956d3ab376bce505ec82775238bbc75b49740a24726d138220d3d649b" },
                { "gd", "bae980fafe83dbb8027f214d4a08ac5e58fa3355bcdd701281efeb186809e995cc26ebdd8a0c8fd7b0ea65e96a22d42980307e751d2d2512486deb52b4994da1" },
                { "gl", "2f239cdcd9c8a4e7d8fc3cc69e156a5e2f1f6007d9c80bce82867d3899f887b04c661494c9c0587f37bbe53380a844f38d618ebe681f0c40f29b5dc39739b7c6" },
                { "gn", "4d6960c46ad84cf460b0c3b621b22d138edcbcf50bb8d703fc638c43246cb2b64738498d42ca0ba2b4c034beeb94d8062dcaca275483cf8d7ab57dab4b9ba1b1" },
                { "gu-IN", "7c775b1d3e81a6e834eae16b78ffd2cd56e0622de8f77cc7ae4b6690db35ad3afdb38ea194d69603bac7840e31ddc1e8fa42cf1fc48c7bda64e9ff06e9a3b3e6" },
                { "he", "3dbb23973e11b6873a3b80f0fc791bdbc06aafa3c0e8a2346e87bb1bc15f975d66a995ae77ab160f9b01754759e5e98d1de3c7b7c0ff88679d340562f109a865" },
                { "hi-IN", "073bf3125fdbc8e9ddba65f4bf972127e5091a95f4523e547bda19b0a11d6ad9c364cfe29bece5b80f09feeac89d5d6bf9182c2b9f520f91bdbdbbaf8266d815" },
                { "hr", "90b9ef61249ca964c885bd754ee7c497bd92a996ffc39d21d44229af153afeead4ffb3032e57026ff9faacd6b07257d53c5c4eabb7f7d9611b564b57ae237cc0" },
                { "hsb", "3134c760d0d7a1fbf97a83229ea94b8686af2a5c7b72945ecd93d2a1daac6a71832212d1bb1845c48fa543336aeca10583627fe27e233743469a267b9032d46c" },
                { "hu", "47445f4a5b4aba5dd0d0e278565f952f2e7296e868142082330f28c70b9238121290e8d4a46adab1aea2fae97eb0ee1987b835481a8cca7b20e1944d4c377f9b" },
                { "hy-AM", "b12c22996eded69a3779069605e912fac8d5cb249d7fbff1cb01e62a10d46e94821b1ad993fab4b607801f560e77a4904224650cdd50ad87f4e22127f5debf79" },
                { "ia", "959b156b8b3eab78d4b9f17f321e37a802e2bf1a201232c741c4560c9131ec3471a490a0ec44df3c090f9be1177cc6212a2303f0b0fec76492a03c3b7c92a5bc" },
                { "id", "ba9fa6e73191b7118c841efce7d3325534fbc933ec8e6ba6474abf5acaa7f117375af0c078d83f7545944e492c11bdc1df53ea2d72526108fbb3a0fca0663b85" },
                { "is", "117c22de32e4bbabe745c78aa9ddfd19e63c069514a371347719fd9f84afac98c14153b8ba110a23dafafaa70fb779647c162667342c4fcc038b570b709bf158" },
                { "it", "b9c810a2155bf66fac7d84c9e47e29fe27cfb803edf6c05c9b914b4dfb1b7254bf2f5cfa879a3c4a765e6451a2fc2ae83f6bc3e8183d473bebe4139b042ef852" },
                { "ja", "b1772153f1129e2b8c0da6b4dc8239fee07fd89b4a63259291cc875945f255e61a4a95b3bdaafc1f83cc6a58c32e750085411528f509be4f00316d29c7845788" },
                { "ka", "a50f2cebc8e051b961db01146e5d6841dd77382b12f0cd617f6381d6eb51d61654f8d18fe7368806e76e81530fcfe980302e88b86edce020d8f3544500f8c1a7" },
                { "kab", "b8fa0f937ea9827215ba697277b03b8156451abfea46d883ef27986eabf6b14982eb1d9fae39f0675b9252f07690ab1d4459b7cd5e79a61f26a08e2e1c181ce7" },
                { "kk", "f1969bc706e6f89b92f75c2b78b9ed4cd66d195a37e92af3b74b635c1629271f9f62d657b7a3098061d075e0ef809c22112a50d91e4bfb10f05023b4cf0f6dcf" },
                { "km", "efa4fce851aca661149286649f78131e54fdab5c088841d9378a9bf347b2ddba5d00d51d23b91e54377a6ae2a5da32a5c4940934ed52e1378b1a9be2b1c2052c" },
                { "kn", "1074251267d9221db7c2b9978dae04f7200b1d5ddf264dd9a8af14c9d0b2266775594fa10aea0b74e95f6bce0519e3a38335c941e83189a60d522fe9b6db8393" },
                { "ko", "0bbf34439b6df242754806bc402c30041a6335c1539705369169df64d5414553230c01f39bd098b7e3760d77bb4c5b90834329091c89dcade9f10361e239556e" },
                { "lij", "9d39b0beb55db87def564717cff6b5930a2b4ca7383c58f5e5cdd57af22393c884d92dfb6e5b83d5a0b782661fe5768128c6e84bb5aa75ff7dca2b769f298b25" },
                { "lt", "bf30e2b9c7dce4b5154f37cdccc82c71a0ec53b09ba8262b839d4cbe88a8481d2502a29227723a68507f40b0a916f683d782dc11695ea05dd78477425bff68f5" },
                { "lv", "eba6561b110f7e40a9424c4da5a0bb29459501fcd88eba0c65e3644d11409f3252cb4a58fc779ce9adfa152f100640e6c3c58d966ea8338b098bcb322746a50b" },
                { "mk", "ec2d2ffaa9903e27663c7f35a202203d498c65eaaf718a0ae4ee0cefee40974c52318d9ce37283a43e1d20650927c1b68cdadf1c9c6007fb2e84439bcc5520bb" },
                { "mr", "4eccb363042a6e955ae84e98033b3d0ec745627824432c07ea9e217ee8f2345e7938a57577c78cd068728b138c41cc579cd55e2dfcca0662e0eb3a0071807e4b" },
                { "ms", "55f52243a593a462b324cf6c06d4cebbf31ff4a67b2d00975df6018a5eadb88d9f83854aaba92be5516194925b80251004c102009a8c4361098a601be71337be" },
                { "my", "d9d2dcf8d5b71bf6ab85e63f9cadf757aba24d2eb415c74f201c1dfd3301ce5d71116a5987589f92ce75455a64128069e85e69a92e2c88e1d482ce6a5c143f3c" },
                { "nb-NO", "65fb51945d3b7ea800b2505b60e80c111e1036eb7645fa0747ba2aec3a9100e76ae3ba0860ed1bd898c7415c2d326c878995ec40709111af4cd5991409a1d664" },
                { "ne-NP", "c89c9b7461f9643952a2108654d1eb7087fbcf56f4a39c7dee4798a4f68c48a97ee844330f554fee091ecf033ffad6cf434f4b0fc56b63f87842e0d23c4ca6d0" },
                { "nl", "9286b965a06607ee42c8a3bb70a24146deba5c1d8b7ae0886542c9a1948153b66218c57c4a36bb7d7a4f4617b4a9133bd6c24478ddd937e18886357e237a5ac1" },
                { "nn-NO", "dcb2a05d53a1190a920c79a7e56a95e092258853060054987526366b8221c39f898202c4fd43f3d0e24869bf790e7f439c0864f8a44fd614105ae924a0d948f7" },
                { "oc", "ac74929c9561f6e2b0a614857cf1e22e8935b5c86cc97a4ae6aa486a140d68fa1ef887072eea00c4f2cbd4f3765d42beadf00f04e2d9150d6408707a03824ec8" },
                { "pa-IN", "09c45875a5b94841e829129ce152d3672cbdf9439d379ed7c0772ccd88b73d36980ac661afac84d4ddbfb10f91656d89aeb1aae4c8e2d7144ea8c6c8a157a367" },
                { "pl", "0b20f62f8ea4076c8f8091faefa46c3acdd8655e118047cfb1376c871b1a7ba22e76e2ed0f86911a82424155ac5462eadd37897466cab6ab4e9513d75f9d618a" },
                { "pt-BR", "eb1f1ad2fed63897bbc8cea1e24e0b8d043bbd3e2c1609904ec76467cde87695f5cde6a70ec83612f55ba491722c2a46fb2be554a1094e12ec7e9af8480b4091" },
                { "pt-PT", "3261160e2e41073b2cee4ca0c3f3cc221c22860839413f95eb716b6515a2e37154065d88dd79e837301ad026839c575af36d6000130cb655742f3b76d9493a74" },
                { "rm", "2d29a91fd00a1c9ce41692075d36f77cbda5bcf9c7441a6b9f1372c87232a38e400a64da4fc57b384566b2eda8f7f626cdaac0ddc9392245f02dcb6cd52c149b" },
                { "ro", "24c1adf3d9eafe6740851df2ac4485f125c2247b48b2f573664bf4e18717e445f734f40a504bbe6b86ba808be1a294b3f1b7a1f3b2a5c2860680f3c0ff495b7f" },
                { "ru", "d54514de0a98067532dc9e9404bea26f7fa8ea19abc0f2369af004c4d3e85919d9e83d3e404c4f29248b55c9b44f1aab1403cfe2f9da50c9d0e3152735fe3725" },
                { "sc", "8e9c992fa85bfa7be00811414d1fd82767df13e89e75de987e988e8dce405a4942540b7c09fd949eac39ce93a5801931f72548352c5d016e17f3476febc09b2a" },
                { "sco", "7ad3ddada6cb456adb6764d099a7ccd168c57be187701e7c1061d1cf3045eb5b6ca65e5d2696fad25df72c6be23b60bfed57f99d0eb4f019aa2620c7022e9a24" },
                { "si", "30458158cc3ba2c828b7572e8c8c9fd4971aac2dccc11abef7a3ea25959d47bd3d464acf6b62c0956a392e9ac9dd38d8bf4b92db1ba4ec7d11aa1527fdbade01" },
                { "sk", "e3a7cd9300137650a808fb9285dd2a0a170f30fbfd82e70714ea2063723514007a9c50ea36063520abc28fcd864572f0c5d5f4f704798994f05c4dffffd56630" },
                { "sl", "48fb8456f7763541ec0e914a8058ecc8f1c4509c953423ae9008a87fd942a7bb5160dde42a010d9385a282b84e6df3fb54ff8cc99c466edea45ecae5030f74cc" },
                { "son", "deb5d3402e2043e62d06f244420f0277d5a2882b994a3bff5718226772f70a608c69f9ce503ae6919ebb8ded7bca9a59214f61288e6b5ef4019f271d00ad6260" },
                { "sq", "3afb449d6b5b2ffd4596c1ef317cd4c9fbecb78420cd6ea50a277e026d8cdf5b31f989f3f86f831fd684573b09accdfdf8c060832dd1a6b06603792fa82b3d32" },
                { "sr", "2c10d9e20a44c3b4167338672d342714a3888031cc45ec2676038658c7c839b50db19ed9861f6dd93f4ed0963f28fc25452b58e5ea749ed1cdd33e6d189469ed" },
                { "sv-SE", "6de4673a2f0ae5559f84faedbfef8893a950105d8ca5d69c9b865313e94822f3814d544e9195ba3d825b1c09881617c37fe056ddcf8d75ced524921f563f0968" },
                { "szl", "a1086486f0f1ac64cd940ce41549c0624aed9977766ed0e7b0a81f1ab392d6d6b9dc4c11a97024897845a921e543e198843fbd47ed6b1b437b39b7708f9ecfd0" },
                { "ta", "8d12b95ab6120f89b2ca122208e459bd1855354efd498b3c124089ba53c658d432587f732dfc97f3bbf1df180cb1fda09d7d612d06421cc8b7b36c00208d879d" },
                { "te", "de3020d8e8f4e340590feb6b66d6f87db147079817303f274e86f37d003369a3a3daf32e5a15d8a08e4b8cd2906f4f823cc07729abfe04d49b2d6747fb79661f" },
                { "th", "a87a5bbf5d5788a8a9275701e65af33a5dfe5c1ff859850064bf22520599e7f6faebd3abc8cf2de473540eddad6131629bfd83b428b4f1389353880c745f2e15" },
                { "tl", "fb1c9a35c75771b9e33f2f8d99411b51fd837371a1e02d23895376b9c23deab63166d08bc1b327779a60148fda1f85091d021dcb33cacf0fcfdb41e95ebc67a9" },
                { "tr", "3ffa9136ec4fadee56720eae7b514023edc358df765854a30ad4756cf7bf7470738b6cfd077cb286365763b657636d0c8cde51c1be929bf02bd6332a911569a7" },
                { "trs", "f3c95cc2f384d80d39eba17ce0b5ef68717a31d043bb520cadcf188a9f0fdf54f6197f45c77f3ae6ca1db15fee050675f166a8ec8df1fdfb63ab4c00efb8f18a" },
                { "uk", "2e0d333183e10b9cfe2e7cc2cdb3ffdc711a20a0b888ceec64b70e1ab6f55be97459f20950017a7e46fdf7517fa7f3a14c9a1e7c436a15c15e9a1ce2bdbad55c" },
                { "ur", "9b2eb48aa103a71859b1b7004237c02d9b25b1d505b21f6921df0cff985bb9cc052758171343831d4f109e898994d5f27f0da80a0adff0ef0677d5a38f5da0ba" },
                { "uz", "c1aa815b40fd476b8837e7d075410e3721e9c5ba65b55d6cd46d2c099b6208818bd74c622cbe1ef555f7c5777a0b5b762e08713b42dffdd4835bde871cde257a" },
                { "vi", "8855600ff625679239aad88dd08ab3de344e46d622a8144486fe374ae4b0f01b9c29c8f05d75da20e27d4d42c4c123d064068b485b8f2bdfa6f27fe8fee73300" },
                { "xh", "0103b16422f8f934cac0b4b4d09edb7ab320f087f3c837c2db9ba55ea8789bbd2a6efb0517c6c597a7c446a855cfc9af10481b7368dd22bf426f27b494753ca8" },
                { "zh-CN", "fd0d7c17ec337866511992b0af6d9f90efd7f57f995be9444657a0e99c318e391ce0b31f1096407dcd1c8a19128cc79d61f384457d31e95de4afef245d39394d" },
                { "zh-TW", "1fd0baec33bfa8bb96e1e1ea9b7a66bc96d6a25a186f36b9cf18f9e42356467d0f867c5f21102b49bcb1d1121500d3aae3925dc20afd32edfed5c84aab18c2a8" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b2/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "439d9404ca2681f79d42e88459e778484f6d3b47a44d642546ba2af464b84e30db48aa80e05bfc8b8603f89b9af2127e676ea1498064bbe1cd2dd59f4eb50a97" },
                { "af", "2e6e0e44223c20a47a237dbdb9b477c91e148cdb94399dea381980f6487fec90627a81beb6ca1c1c3f66e9074ac008f05baa7fa756aa88f2da69ad7fd9cea6d5" },
                { "an", "35141751993905edcfd82f9d7822c950d9b4bc001429cf291257d117f5ede6ba8413bd9a7ef2da8ac39944e120edf5bf14750fa6bdc867abc17dd6099e46241e" },
                { "ar", "e36a99f77679a6494e2c0562e5ff644c679e7bb59f2e589494eaa67daa7bdb0e62bb98acdaab04a8766422d78c601c43ed3dc72e7284d98bd7773bbeea021946" },
                { "ast", "e06e45462364f822c93c6ca13820128414b188bc7e3132577feeb2f943e3884608e09f1d7fc36e47d28792590de6a983c61ccb7abde57b5fe47b5a787539fc4d" },
                { "az", "778c8fe2e10a9d3b2297ec2839b367635a2efecc0a9ede7ff94462c08ea39d9c656cc92721ce1dec619c70cb2ee7ee08c9510af4010ee73d3381eaf814566c39" },
                { "be", "e793351842dd7e65203b529750ddf1770dc994a2e33bda3574f0805f19b833f2ab6bc4bf24cd243c8335685dba85999bd85d2896b53508d4efb74db24b0cc377" },
                { "bg", "ddd636cc4e5c4611ca7cb401580ca750af78bbfb25338e2a0503bb552cd9490317e3437808900d2c0807e079cdb96e9b81c42029b5b5bd20a65712463933f265" },
                { "bn", "44277d884905f12d56afd6fad02c2eacebc9481e48614e44a8db131a4271bb2b402b2dd6404aede75f439ac7ae2c499b92dd97978ad461c370150f4a0bfdcb09" },
                { "br", "6faa7d3686629108a4480b1937a218bb2e77358b215a8b2d863104c89eaea90c8821c7324bea8c4e5c0fe41de1082c46106ceafe1b1fd20de9ae2c34284424d2" },
                { "bs", "3e7f14afbd5fe364f64b69afbcee37f6cbee7c2dda4788e9ad84f145de3d28070a2477d1190555aafb09dde2f0c3c6f5d534a9534fb975d77737aedaf37de7bd" },
                { "ca", "309423f06fe95e983061561819cb0a2a5f233abbd7f0a0f8f5e1e69a7b44e4fc5270d1bd42eca0527a472909d42795206a256d420369ab983148219ec7efec99" },
                { "cak", "196759d7a656c0b1babccae8797b9376e2fa881d056d6cbef6b433f81ef1f290c15a0808087c7ea9440305dc02d8737b750dbef3f6145f6d8695ea623b11be60" },
                { "cs", "b5404cabd929373ea765a7d920319f317b12c702874ba9c57796729cef7b04c031f9d5091679067ec3841fdec31fcfd74ee0bd4e5194a2724a4bb78dca594b03" },
                { "cy", "f48c9f91fe141f4dcfeb3808b8d44da8f48497829be82c4c903ecaf55add343be1bfb73dfb3459c78ededd4f0d709a54a33c5a4789f80e5f4c450ce3ea0ac518" },
                { "da", "78c671155a2681d8ef483f8a6afdc7ef16b470da71eb830e7c03e8c3fda9dab32da0d768ec4f1e298150162df738e7b8b027de3c26b8cd0409060027568c7ff4" },
                { "de", "e5d68259ed14f4f79ff990a134d9cffed9d090b61a725377a20e8b41942a1d4836333c17ef8f25c74e62f708fc733a3e377b88ec9938239107eb35080e9a0c7d" },
                { "dsb", "c38acec35a55f8d20686e4289a0d59a30d52db1350bb870949536334c6feb87b7b7ad5790de710fdf69423be8647ac00ce5485ffb2d0963c842ffaa3147a05ba" },
                { "el", "e984253206e3fa33a9314de0cc363b6cb8caf0cc0d7e82d0abd9abeeeebd478602a57effe9b7ea3f0143f2d5f2256234b1b506f94fee5ecea6539445e58b44aa" },
                { "en-CA", "8f78a470ca8e51cd6bb40bca73c0fec37e15a27001a782a4a56dd17d2858dfaa20fe22ba1e13928be6610bb17774a805590f7e574f1b173965d1030714bda7e6" },
                { "en-GB", "e85cc0a12d561f8d8412bcbffc69a1d07bdb8a42a7655796ce29863c990462477accbbabfa420bdd4c25e8034b892b77e1fc985dc85666c7f54ae86c3b45aa61" },
                { "en-US", "7227f4698091deda046c508da63cad1bfb37e1578c8acbd7a7698c06988da8d2bf7d62180945ea1002aaa9f57e1ba7cfaa51b562b3880ddc021b0877dd532464" },
                { "eo", "9a406770c0b8d8aa0c7a991406e0010bfe71c9c3156fc29a1c11a8f03ec6de1807dfe87241d56ae8e485996e95aae5848f6cc836da2551a3428fa92f01f9f2f3" },
                { "es-AR", "95f59b2a02892932ec5f9a694160ef0ba9429bf031340e2ad706724507b5aff02e26a951a8f55e69f6b8e8a7c3891ef58007f5d31000888730d65221ab44e64e" },
                { "es-CL", "53e1db80c61ac2467759630a8aafdbee0c0baf20b43cc99f064745ba0907b865b1ad87e9b311df02430c50703696756ab6d77bc980b28a149d6729921d7544b1" },
                { "es-ES", "8f96dcd61677dc7c45260d298f4c45000a257865a16545e47527be3e4fa9789848a89eda045202d459795befae8f0d34fe4f4c7467a352fb029d1b0eedde8520" },
                { "es-MX", "ee649a5989f1cf75713d33c9dc60e286eccf4bb98a076ef27ff4159647a801b814914c59808a34db79af260fc58c68b6080543e14785f6722282cc47ad7e3db3" },
                { "et", "7b81f6b9c9d67070c0b26c3735363a275762c5c413e6180d2b23ab7500427064984b0131191de80423751bb791aad862f4d44953f69709f9f8e8843002fe79a1" },
                { "eu", "6d2103c4315a958c5ba769bc5dce2bde956d67abd945170aa0416a0ea28dd18fef3449a072be2983494934e0fb23c73e67e763fdbe3a286f447195ce949f3fd5" },
                { "fa", "35e776ec6cb1c9a2ab03231fce0e46ded4d684e166a0e000e984a35037d83ddaba593d736a82dd849034dec676067dc6f9cbc5afa1fa5237ecc91565ba4564f3" },
                { "ff", "8ac611bd2191716e0083854e376188dac9ec03da77cf86e48851c6be7d495d81a2bb14ee9de9944d53c76cbc8d63738e9aea4fd04a457ca7d948d03ad7aa22f4" },
                { "fi", "b10a14a3664fc3957ea788f9353ff5fe6af4b409d2ac65c2d13b7640e97d41459eae37bac655545a5d416b2b5e4fa87c1406a30fab2ca786af97496853f453b6" },
                { "fr", "49708bf1aacaf44f18dd5214b90ae1f8548a83278de581b6e8513cc65528990197e661f0e5419def543cad12a5e9e21819b08e441b899a280ac7c3f2c106ce58" },
                { "fur", "0a47ce65d67f747185d8e63d59892693c3adda09007c853b1e674fb8d8b702b82f2b25cf7a13e306bfaef82adbcb7546140b93fe580cb0ad4479a29e9ceb986a" },
                { "fy-NL", "4f3332a6ff50987c2f3c04bee82e485b22f18d8d79316a016866fa2dcd34eb535c9bc2a91efb55f433963a19af8318801726d5ab553f37438a3a86f20d5c96a5" },
                { "ga-IE", "ef510b4c98e19e12a9059391cc4bc8fb3ae8412610adcae5d5833f2fba0f61497f089b0cddb2a8051f17ca710b1470ed26239d149da9d5fe7c8c25ced5f07504" },
                { "gd", "0f153277116af1e14489b27d24536a6ad41b7cf131ed036dba67b308941f3719c3f6eec5a59d58261f8ed58e8097c5f289bf138df0e86af2620c199ed5a09442" },
                { "gl", "80f1e5b21a54fbdd7a770c831551895d48c34ac2bdcb0ade0cc07e2a89d0d886165f91fb8c10d0306a73ec182c32b538e558467850b1e1e83e44432e3d6d637a" },
                { "gn", "d86d4583c01295f38e4c2ee88d6e1229b5b18daf54be93d3823babc528bc31cb549f3d6a99f5b80345090a97e6a912c85b3f3a001233b9b222ccc341ce1181c3" },
                { "gu-IN", "37c95147cb6d90d3f03bd408d235385ea23aed2dd6e3cd8790dfd2ee9e587950702fd27a7cd5c6599203b18bc6a68c63c2812d0db7ab93a2963495539891dec7" },
                { "he", "3c1c04a1f5b9823027009635d76de4fa5a22ca36da66a4dced39e4e99061f3463748fcebc83bd0f93fe1c8a19bbe5a9ec64db667b91d1d4db2e17c419670dda7" },
                { "hi-IN", "573981c20f2204b44861ba2773a84861ed5857c7b9191cdd96e00818494e74a0166b796b1c39b65b2955513bb1becfa8c586a22dc4650fa4b8e8711e3a4075b7" },
                { "hr", "8b0359c39527c707923317b8b9fab75ed34ec7645580c574ffc5a7c5d1b1ec4b33f4c3a542fba73122f2fe18c725285936567007a74b3edcae95febc1cb82a04" },
                { "hsb", "3cbfdc64b5eb7368e48ab8fd2350fd81e7c37383206a4bb0634ad123c0208a08d75f8e9ca3af63eac155d2b1c9ce498906cf0d88de27cd54832d414fcb9a83a3" },
                { "hu", "1e6246332543236c0bf5c6bc663a7624e60b46d23516cd229f92c2851c7cdbdd1530f17b2e4c9c04aa210170b75edd9a5f305d5c502c2fdd64f65c87abe151a6" },
                { "hy-AM", "d36f772ee3c6fe4c5a860dac765de6eaeeac3943e0201e1381bfb2ade6cdcb456f15c16f7705d5c6bc4669e32dcaedfe9f6f2bd43b86917b1e520d315816c914" },
                { "ia", "3d3da7b0d7152c49f3f36267a952488bdd4ab7c37c04df29da177b04df8b75c4b3dd81a7d13d7a596ca28f4fc76db51f090959ac6e3f8d4785db81e0e3cb22c7" },
                { "id", "c33bba300ee2cb11850936c46477b8090bf710d6c72c9d0326dc5708ffbe62871cb80590efa6c10d9033c6b89a83774d99f6594be1d9c439ed27ea39403b0ff2" },
                { "is", "c578cc2ac95f1f221b71f5a1b8c65173982ec8e758b7ef4ae6ecd69f884ba2e7ebaa4605dfe98eea417503b818b0368d5980629d27107fa5e536b50506079ed6" },
                { "it", "a7dafdd2b00c60be479d2caa496885c12ccc731c1a6ed6d2e9152645257642aee84de77fecc4768204c7b2b9becec449282e960ba4aa53e8842bcb7c73341662" },
                { "ja", "eeb404da30f98ecbe28ea9a7efa58c54174ba65c9af9e5ac7c5d0a42c48ef70ad59265f2a1252f0cc304b4e587d61c73ab0e0c7717dde20c135ed56c6f4210f4" },
                { "ka", "da3273464d9aa9580d5c1cfbcbf6c48fc346ee0bfa45f3051b721a8e6bda29dd270a88bfd14fa2ef00fe754dc01602c07725259a78fee806e2efa81c11ee6f6c" },
                { "kab", "6677c978320aad809dcfc0e04b5919d4d3e12ea8e5fd629af3efbda13dd442b22e183227ba9057642f10a7ad52f5516fc95c45c1fce093f5feb0fc4e629c2d21" },
                { "kk", "837f764dd7f1359e51bdf5989ef233ad20944e6217785d93a351a56cb156e37d319c18bb1b809d75b655bba954b6d79f89fcaa3b9a581041045abb9d075cd3d5" },
                { "km", "88335bb84326cbf7cd3ad55a82540f0f6669494c59862c9b2b7b867617bed8e6cc5fae00894ed10ff2794dca598932ed1cb161c6c4ca92fe01e5c584668d095d" },
                { "kn", "cab32c1d013b3b431108259c04737622f7a1bc3d3da35a81ec3e8a216fefb499c41f6cba9ecb045d8e375e33a8f29622aa889404a34b147e6f863ad0379be443" },
                { "ko", "f44b75a0416f43408b78ff0ee7262599e011af2d3873d7c66f22c34ac5095a6503be44ec4c2cf253b633d61d981b74a4014c2f34846a7a4e0e5e1236e26775c0" },
                { "lij", "8df5bcd54c5b16b333476ade158c590d3f221aa7fc1e55262aa128acee1bf8a3a543380dba19ac2b75afd0a5a5208282f1a6933945c1c622d573082960ac03ab" },
                { "lt", "242fb0660f517cb4c3c78ce500b1c010e3e6a6293728cc3c76c89a183c9b09eb3c1b734ddc8a5d516de113996fbd7a210c591076d2e35acd10bf637afe137686" },
                { "lv", "e807d932d8019d88c8666404922ba0925f40fd444f27345b104782637b353a452cbc62c7b4c8e14f5f9465c7090c377129cf06e59e19d734a4c50a6efb81873c" },
                { "mk", "5bc687aaf6d5ed48959114977af3bf7af44cd751e7380e598c403dc23748fb878a3ba3458cb8f6eb147f0a559c642c09e8eba292c2fccea03f672fd9aefd9d30" },
                { "mr", "1c87d6ce09cb5f60d25c2752ab976b41d60e799c45911c5e68336ee0f4d3c92a62493184d224a35b10c361dad72802995eaee537bce31a2cecf1d41cda2cb04f" },
                { "ms", "7337ad01fe136101cc2edf9714cc04410941b3e45f7a6f293dcc306f1678343145de76411c4e8af2091eac240f25afe8d13bbbea4b4ecbad718738c5504dcefb" },
                { "my", "08ede5699067388194eb9219a878eb44eb163144160b60202b17c1ce40abe46e637f6848e023f90ec3ddca603dae69ba3fb40c80889899d4fa11d0d8f52eb9a5" },
                { "nb-NO", "34a9883af5c0e96829a112c15a9696758f88344e23ad8797b5e4d8e8939638a7f83009035066a2e64497ec53cfce1249e22d59bfae4f1e3bd710c21553234c5e" },
                { "ne-NP", "80bc77cd9e44b8e6f20a089ec2fbd2df7fde65d719262237c7f801eb14fc5e5ff2a3ac21c960d713b16c936b85b133d91e72fca86ba5b016d416c0a34b882887" },
                { "nl", "4a547b1bbd1a2043c96bbca18a28228ce0f946a83b0aa90cca78588722e240a2e13cc2d53ad7c68f8121f7a35c21cebacb19309a6db002599cc7d7a25c031386" },
                { "nn-NO", "a42a5cb5f09ea97de06ea3d3d3217f884650ffed594d1d5bb5d3ac8a8bfe3e0b0fb21558a54f262f68be46bb720368cb0c9bbdfcc54ffaf02be133c84d2a7f58" },
                { "oc", "89965dd905825e6a60a4b1c9a6341de3c8c307d520371d46eea9b84ab2f5e55e2e2ebbf0d75c57867f5ea7086a380e603dac87d1849ae965d1f08e4354f274b4" },
                { "pa-IN", "1852c71616978c2e00d2ac957b438b35eac01842c2ab4c1039804e6a661a3f5824a751b0a2cfb5705bfe8474bfb14f96d901c5d9b26a8afd5338dce57ea548c5" },
                { "pl", "1214e91f2d02447f2ad0974cad525799dd162c828aba1b0e4ca63e0b30a870f3ccf23bb31bfff926a91547e3302827e5fc41595bdc104866d04619aa900947cf" },
                { "pt-BR", "3da7bb6f8451b696d93593f7180577cf6942370e874c100360441383526596d350dbb6357479b6cbf5c27939ca7d685aadfc28fd1858fddef422367662af5df4" },
                { "pt-PT", "cc1eee7a34f45495bc45ad3e038822bb80d31707aa1e7fa676cd0b06a6c588e6ff3869e60ebb441d1c5c897a6e6c3a322198919fd05cf66ebef8fba0abd36b0b" },
                { "rm", "6f65f7a4cf86916e6ba506cf573d4972e15e1a9e113274897cabb219542f5a43d56163154b52a81cb5fd48f65822a4eba37ce2120a87f38a2d5b8968accb411c" },
                { "ro", "fcaaf175609bd61baeaaa1e9275f1204bfbf9ca256560df21bb3e4376653167c15e80e44c78794feadd0bfe0b581fe7486cd76d83a4571424966b3c0d32b419c" },
                { "ru", "457e95d62bb69e74911323e4e4bc00bdc02a1b799e6c43d0b76c50ff0067b021e3247274b133dfc7f69bc11d2ead685ba06a369bba3440ecc57c95b8137df663" },
                { "sc", "763c1b019648e27dfee0fe1707ab19d3b9a6ba63a6ca281afe343caeae83d8d482531ba2351333c1ad2bc4d2ee5b41d590a4cf838339fab312eb9d5343816d54" },
                { "sco", "48a52733d3be9dae4647fd9705dea6bab1ca0a231153e895c87e591fba7100dd9d92d96d3338dd329b0d79ff46218aff6024b0a32c3f281408b3c9c8e8bad26d" },
                { "si", "5d5f5c8e6cb3eb1bb39be7cef80f6e50a5bc2c85af5984c5a3b9a7a8da32e9c552fa2725d4065ba44259d2c45eef88078b5e5a8db8dfbb0b79cb4eb086fc1fa7" },
                { "sk", "f08498231c7f896665ac6a91588b86c34714eae4ab3bb29cc945b50303142c8a6cb64251a81a24163bb42c0fec9b03e4f696579a67bb6e8179305a01b80844fd" },
                { "sl", "f18628f33f1a69883a009870d30cbf7b66be1f61d1e0e952aa393a4b04b8957f4e3a78e8fc805030adb8290e45413348e205a0dd7c2c1043e06b4bb1ed32b971" },
                { "son", "b9e9d6e18b960eecb1a19501c17a4c5582588dd61dd530b5f9d0eb5fc6267632a70496c3384fc227f28939cf4277ac72da04efb94287b4e458749e6e3a8c4a10" },
                { "sq", "84e0ded55a5221992f4ee326a087b386f9044556811520bbf4d84e2d730657850ed73b0c6b84a7bbd453e31c7984a6ab0eec73754bb706dfcc34f50f91772d11" },
                { "sr", "305c796fa83737c434db4d6bb68f45a95f5f8fedfb9c4dba849616115e754c7ce6cc6a1285119d55bf416366c4c7328f054748add003777009ed51e2264c6cbd" },
                { "sv-SE", "5da525d3132ad7206e84ed430e96f2667f7a5219f12254149d5fc8cc98756ce3ceea36a7a4665020c173955ed94419206712fd7e1a1953631d2d0bc5f52d5329" },
                { "szl", "f552af1a53a2b3ad7f668d2fb7f0da41e474ba0374d2c46c5ade89d89d91db33703ceefab64b2c24d29a3ebf536d794931e59c688ab8b7628e8e3be04b811df2" },
                { "ta", "01a12e2e2e5ce8e5a8fb36791408f375922667b508d64e31f00a54143cc916ae7c59f7159f960e6078f6c7f44b1bb7e3be1811f48715421e27acf87d31e2ad4f" },
                { "te", "97e1af4840f4090faa678f78ccabf7c2ad4078c43a3c3059053c046972f8939048601b412c5cf0d83b0d4644b47e0e780fec5fdcaf32f228d000b2d43cdda78a" },
                { "th", "2a93433e027141d11e6ea4009442c78352cd41f73d9d616a6a2276e9a9913cde35011a1ea3cb5ddfc5ba0776b110110db43acf86c5bbf24f271bd43bd64b4e06" },
                { "tl", "a962f7eb73a7ead5d6844d0d7ee207a90693799ade12ae480ee397bb74008d8e8a47aea6dfb4778d88dfee4554e63065a390bcb1a05badf859ba6d431929233e" },
                { "tr", "4d050248e8cf4df072f7deffb8bc096c4cde53e8c890adcf870ecd0257d97dbb443184255ef85ad4cd24ed13ab4c581520d7d3e8265877f42894527634a2e5fc" },
                { "trs", "32770f7e2940637198e1f23ded133eb88343f4444e8f4279cb6b6f75f8a41997a25007bd62d241850595983f2b63ff42a8bf999b7df87f91ac0bd7633c25270c" },
                { "uk", "640cd2643e8cc507d19316959bdde6e19d9851d34880569ce2c767270dd0569d03ce45b959d3f4b6458707a10bc29e62ad95bebc4c59a4680ce24c3854b99926" },
                { "ur", "c717ddc1691bca5b9dc0b63a349ace9a38698b923e524f47e621377e8edc61b716a09d7065bf8d44491e8dc87690fc395cfedce8f3ebc7915f87c7da2c6abf4a" },
                { "uz", "a9df7328ce459d26e2057b223b6bfc668dbae008d00c6c40d0e23bac092c22d3c70bc9582f1e7ef7f637ec9cc9cf3e42ad80e52b941b28b0641a040dacb5cd1c" },
                { "vi", "013d9c9ee4309ecc8a6691c0f9efedb2a00c8824b76f25863dac8b30228b4f192de9318d802f8a6f7dfb8f921b16a77a55b1ffb916dedb7e9826d282b8a2113f" },
                { "xh", "4e41388fe2e29ce0f1f220e7b0dd5b133b9f35ef39b66f61f0c820f75cdbd32d430897c16d33efb46f577744b9be00674b6fffcb2e68ccbf0c66e38826c7747e" },
                { "zh-CN", "ec52973d78c090f98a8863cbee63686ecab447af38f330e6110a7381a74e9f1e27c7cb7754d0e3b62a2fa52753a6f91fc795b25e037a2982330887cce067b508" },
                { "zh-TW", "2316670b3147d712daa2d4e140328a157f0089e816da86ce38e5807c15667291a5cc175587fec6158ecc83a3d219f0434a8b463fe0e827a9eb969bdba354a2ae" }
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
