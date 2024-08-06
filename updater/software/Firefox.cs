﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/129.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "c90276cc618985f7eb8a11eab70a8bc7bbc27df0b2eec33bdf633f25646c84f8802f6c026a8b46530b50b27da8142ddde04e4bac23ec5c96bb9c8b9da3b97e97" },
                { "af", "222dc904c2c34353666c902f16498cdee06538ed5172c4498e8d98d80144ab0c1eb6b5a1dfe9d19bf729bed5233c9d8de7850322c9162ba26d5861e43d84f157" },
                { "an", "00b6051db6564e44522c56532dac49a8c9c95da2be4e459b539e163732839fdbb8f2529c7632a7c26f6b0509e5f85a0e6fd7f95835db6c983db8f6fbc91fc582" },
                { "ar", "c8a3be1da4a8de6f64ec7c708f06825fca2ad7e0a3f464b60b53fa3786467d8e227123460cc94b980ef05ed33c6b69ae91296234a646f6398e7957f219e501fc" },
                { "ast", "14923d3ae01845218ffa0ed60b1e3fd177b7f199ecb7aa1dc72b1a8517fa69942829ff7922b66d80451ff49816e6a75c060f3e16bd8f9727d9b7138637e95068" },
                { "az", "2ed08fdc001e2b358a16b917e81816edc964684def810bba322f1dea6ec0ee1fef86760529ef1db58f9d3c1928ea090203a2358d8be88d82039f175a0342f87f" },
                { "be", "5345a0977e2c0b424b7f8d92b6f54e3d96334b11559dd99b2c9c747e61050ce5ca082109a1f96765d587038a880709e9bcb6c60ab1e9770bff1d41c754a26b56" },
                { "bg", "7919234aa99fc37feb99df77ac4784df52849d89415c57acbd72e37733ffa5540e9d4ccd726165047f64e7fae24b9c84ec5dbc2454156e9ea081a20201659a6d" },
                { "bn", "d38ecbf60b6de231ed78d5c398523d49a87446b51f42cf956eca5789e4071913127abbf957778a8f3e56c57d27208555a9a37b037f85e0bc548fc449900eecb8" },
                { "br", "2fcf9bb7d24f783cd1bac81c668b751a4a93ff26c3b2a822f68c7b6a7975489ef812ca45088211818aa0dca3463862f206050547b95eb1e03786d5612a8b5e7d" },
                { "bs", "eb752a8d2898ac61f34586906575da1f1401605c023881dceacb77480ac84bb7a34e47d445cf753d534b120a61faf119aed763c4ba67d1e46746204c74edc8cc" },
                { "ca", "901be349d319e0181d57b663b3a7e87c6780e1fd422f5250b646464eed52368623fa9ebcc9830c937ca98458ef2ff2b6f7381dcd16f72090d79ca6a99fdbf6d3" },
                { "cak", "10fa8ac6112a7d2f6e309493c0030a8d41516b1544e3934ba00e79472ee89d4e9fda80bd83ce2eb7672760053fcfc31c4bb777705b45cab0dc0142de9f77884f" },
                { "cs", "5efdefcc4af45def5cc1179aa202c8ee17188b5ed947a16f3911885568a4bbd422f986a66857ae676c50a58ca66ad1fa39d1e7fb5e3e988fb6f857f0c6a24b99" },
                { "cy", "5b7fb5dc68af0768bfd9546cec7a715655207a4d8091e9927b760a5130ca0e7566f9ed468da3005644e36befef00d11efc968de434d8cceee0a3b210091287f1" },
                { "da", "93c554095f53c6cedef3ea8c4686d2a351f088bdc4b0c50b23adf1fe9290e35fc89049226737c1e60f7e0abc2b3e7a16f2b3b057928f210fe8dcabb26e490e06" },
                { "de", "e5f5f6841230113ef869880009a883d1df25d3e6ecc80d43e1eb6a21beb94e9e4a74e85539d3ebec643d85fab20c046c015f01362a712bb8e1f55b01510578d6" },
                { "dsb", "3406f1410fb2dce3c4c8b248432a36606eb00ec1765b6122696e95e29b07ca6ae581fe5d33eb531f6dd16998bc864732670824f4ab0f414097789295057bc807" },
                { "el", "a067719f30c3885a063a0fc222087126fe5e0185627bcf29e710e5ef91e268ff614edba680d4142f289af9d7d9bd37ddfc330eee134a8384066e81e1b7beb0cb" },
                { "en-CA", "d3a333a370551b445fdedc5a3fd505f147890126757ce5b14020e9d9e4f00cc2f3d6fce1c7766f6c016910350908365f3903f4c46f9982693519e38997a64bd6" },
                { "en-GB", "7f5952011c9e569affe8361af05a05fbc6c5fb82dd69bcb398eddb8440ddbde122c0340a89b55e153e0326fd30da8bf0107bb4b8517e801af95cfe227d69fd82" },
                { "en-US", "fc7eda8fe1d3ba2f9a0b7ad1351398372d90bf4d2b561dfcbcb3960f4f01d4da00f1e6a47c70c73092283bbbd9b068721038c5c8e723888435e8a5931b7f3527" },
                { "eo", "b1748fb7f48222b9559c96020f859d4caeb7d12478e5caa944042ac2487ed36876f0ebfbd0820e7c8477ec3a87e842833add7f558d82b16f05fea39a29304a16" },
                { "es-AR", "83abaff9fab6632797fac73bc3e1b6fa134bdd3816429088dece1c63cababf3a4e3d77204339fecd5ce2932d80ae814844da8014b811780f0fd62f5cdd73016a" },
                { "es-CL", "39091db3d1ea8078e2d7a0cd06e68bbd52b91538fb79973acca28bcceb0392f0d343a06de803b65e55ee83b7a80454c3013ffbf8df2f0c375597d3e84c6e0a35" },
                { "es-ES", "4a617c1414c306eebe0e6ef470a00b7ad181119c167c37f190e0dedfa023e64ac5c0c6722e80d49a9372005d0075aebe890c764c08a6871eb8dd36aa9e58da98" },
                { "es-MX", "510ace44cc0d40a390f4ec134667a6687ee6abfa867b42295a5538f5442dec798d4e2dd06baebc858b276862bfa1667b71082a2a2f5bf906c817b9bcd2af9501" },
                { "et", "e2eb2510923e0e2e82ed498e6684f248338b2a04ff333c81c14a7db67ef064cf4c6a6239d83648b1e24c41824dc58d7d3684d7a3d0961447c48239b2e7e3ab76" },
                { "eu", "85a45d1750642cf91879cc82ea2734585cd4c3217f7d9e2162f6da241d7a5cdd4956e5dbbfe0d8c727023a16198c21654e970ca2e5e640327496c9a69f9c54f5" },
                { "fa", "a88b7528d4540447f6edd39aa59cf68c729709f5c110d07b6106d8acb1ed6fec6999e4a227eff597b414ee241db4593d4e5b1511b9573164d94cb9be6ce1c645" },
                { "ff", "c582d68d131d871ab66670220645d9def6b263adf37dd32eb5fbad85d2265f6b9054938110fb35206a8957f02ed2ce21c91693159af1a347ad34f079f415ac27" },
                { "fi", "5ff94df1b42def72656f2d510533db76dbcc7361f658ed2e22fb18abf1d42abee14356ff97b611d9f2d353857d9f895a00831f2c338e39ab7187e17eb075818d" },
                { "fr", "2eebdf3b9cd33410569ae7579ec5922b0fabb75951c1178d11bb7a7f3c231d421181487494282b41a92954b973eff5d3853a48cf02d09d77ec4672f3e1badcd2" },
                { "fur", "8fceb18fdf7e6cc73c8e194c7107f827ddc597afb18855c32756f91230753fbbc08a41e69d6a0e30fbe27d530543c6542a8a8212443859645491758464b1b537" },
                { "fy-NL", "d50bf85124fdecf0e3ef6315d04ff64620b878bc70b594ee00cdf383246204573ac17ef52d026840a39a411b79617d9e24e8a34d9152261ac509d66e6e8dc3e4" },
                { "ga-IE", "986b800baf36b27f788fd7fdcdb44c2d2395d4dd85364bb2db4f8066754aa5eaad6d79e6b2f1f9f5ad2ec3a8e626d18c9de72a3094bda0e216a75fcfdebcb71f" },
                { "gd", "71fb6a40a86e5bd213028879bbfcf395907ce654164572e50646e9205fb8d6048ca6296d74bb9ab845bf33c215fdf6b09ea803ea2485973df982059eeea039e0" },
                { "gl", "5a8b7a23202de76df8985629cd9372dc747402881192de148b5bb89b2e91717c0cf07dcf987ae8b7c611fb7458eb3b214e02a087876903af554b99dbb193f181" },
                { "gn", "04fc2828de76f667abca843c99a3efa4d76a3c626ccedeaacf8746474cbb07d94b4d5fb52ba5eba5f729df178bbea65d900b739ff6e51ac9f748ec09211906f3" },
                { "gu-IN", "231f17d4c5c4496bb2e10e8209a3b7377377d9c8f35a05ba55f4683fbb10827c8812c49e91fa8e50c9f63b550793db6801809855228d55928dc3a3ce869cb32f" },
                { "he", "95b2af35e5b5002084833f288bdcfd38bfa3d2163e9ddcfd28422076ef104323a25908c493253ae6891f10932529f44fa099e918ae2b360a1ca02cc73a0bfbfc" },
                { "hi-IN", "45be77ba63f78822754ac743cecc1919a6a3d2bba78b89fceab062531f77d8cba8cfb05251eed73d6e31e8a5877f89939d1f4fc175d456d31d242503463bcb30" },
                { "hr", "5f40688a555bbd693926a3b3712a01f5cf9bc1cb31103b1f67e0f7b20289165b4fcf51d1d74d4b35705881520481529cd9b1ace75c61bd8333cdac072a3c8e45" },
                { "hsb", "7ad6b4147966e1a6e300aa3281376c2245b9687806ea61e4e1fc93811848253c4c69b6a25c6dcb0ff07bd84bd08174a9876d974ba2dcc1565b25fb858f8f7059" },
                { "hu", "6177275cdd8988e10931b1bdb41a6dd69cc56fadf38637f063751728448896a30d347c8782e35dd71a6b84012ea12d33b167c54287c0b9331179243bbdb319d2" },
                { "hy-AM", "417a4cd2376c30c814830e424e5ae4f91967fdf799f80f4ea740b8d329b8d8bc6fb68e4ccbc4c022f30058bc7e14b0f9baae2d5bc466a48fdcba29d2a477d092" },
                { "ia", "0370e16f8b4b5a5e3a8299d4633e5a8bf35e8d6efb970d6c1f446ae80dfaa7b3d88d20267be19e0f8ebed3ebba961dea04480d576e22a43c0c0bbb005fc1f20e" },
                { "id", "5ae981cb67f2464c106e26b02088082a6b41be5244fb9aa018ed31b82d3f2b510fe1bdbfd3e73172dbec3038981a332aedcfe61e240709612aebed0da60cc2c5" },
                { "is", "02d2187aad5136a2f5f62ec6df01a53d255fe5590bf56507dc5fcecd054f08a59394b91b74846c100714b7287595f5f8fd9c0de56a8252db754f25fadd610e00" },
                { "it", "2977b407e622517829994486e7933fbd73b137e5ca57d2fbfcf6c79fec27ccb468fd73a38508e352f852d8d5d5f2096b6ef2da663e2bd68b1036e77d150aa0aa" },
                { "ja", "f3d34a79269164244c0035acdb6ffdddc35092e72b4770d8d34c5252e767c6ba9006a478bc4cc9fb91fa381184206f80763ff1f3038427035111ff0c93cf6099" },
                { "ka", "7722ac567963bac232c9788adab0851a5ece145aaf0d2cc0b193fd059ad59dd111293dd08c1c49f6ecd98f62eb66c25ad4997bd3b38fe363df8a5627b12d4748" },
                { "kab", "219678ca70ad20c9cd3b349ebe0858262ebbb3f81ac400bc9199ee74828313c4b66614158756ca273885e4dc1d18ef8fe458d2a233c6758a0f2fad0898517177" },
                { "kk", "a0a3606b4c0efba1e4d01b9216d6af9ed69f2fe41e6193e69789215b9b06571ebf0220a848801135896738ba21e0e59c209a68e8525635c358cdcf0597ee3cdf" },
                { "km", "b088751bcebfa9f015fa55a33d4065e20f78ef0257ab1e58cd7073d3511b47c814c70fb59b19bc6d629a26eeecc71aa324bc8cc1bef84763cc032caa3e69bcbe" },
                { "kn", "9f0d47d944af96647bc05cf59d5b73ffe6919af2aa19bbc3bfdb4e76868a8896f8b84302207b2345afb25c109fdf7f031b7ed001528cdc2aaaaf8cdc6019671b" },
                { "ko", "25f7bcbbb5a5b1dabfe876a0666c7ebe55d8a485b1a381018ca4657f410f660f967648c243a496af1028d4618e79c9d9108012125a7c52cc3d57ed686a4822f9" },
                { "lij", "b12f28bfcc4677e91bd061d09aa32b838f7c1f41b5330145435cf3058302efbb521106d4d016cf987cbf0ff210983e5f88945fa3ed8bf8c6a3fd46f690e215d3" },
                { "lt", "c139bda85488e4cafe19105563089f4192bc88a7e6d3b95bd9022ec8dec4917e4601b26141a09c19934ca33352c176f4f2f6e820373b46eb2bf0ad6b3cc8861c" },
                { "lv", "57557bcbab18ff1bbd24f8293fba444a6138dcad609fc0b39df72b5037e4a68bbeceeb0603e476b5c230dce2175ce20dea9f3b9c6c778645fadea529d4e76538" },
                { "mk", "ee0fe02895bd9ae38aa1e4b5964bd17df70f85339a375caca53a43c5a8ef1ef88ec4c5078a9ac5861b648881a58d3ad53b7dfe45b2d8437320ee13145c9f2878" },
                { "mr", "6541780eb826a61969e72f4ec21b16f04add48810267b92d0aea7a23108f8efd4f433e3704bca6e8ab468088f572f3d6cb1f794cd8d93feac114a54291bab81e" },
                { "ms", "4a87f16a4456352fcb092b61e3020dc52cd6787b0412e2f361e412a02630e69be8e6fefca51df7b88ee05c55859635087570305736e3f2b6ce15629222e44418" },
                { "my", "f8d484de5e683b8e57baf5861bad93a492bb249741b39169891b105d8aacc38a55ee1d86fd51035a482738c3e6c9694a34708668fa6eba1a5c4dfe0534bcb4f6" },
                { "nb-NO", "e7c1b9dc139769668def41dee817c240b7e840ae717e2b6b15242eac1c0c77d9835b83d177ed0c7084cda0e8ca1ad70f271da2cab15768eee4c485dc6ad0f384" },
                { "ne-NP", "bd3c3d290cf74a568ba2d8d3b0aae063f66aea565110179656859d0f06be290acd41b380a683c5691dff081f2a2adda80931dd2dab935440005bf63bfecd93f7" },
                { "nl", "d7b5a5529ccb6042bc27cbe2ef0e297fbdd32122ce90d9655a4e49199a2cfc8e0757d43642a823a44619894ef2fe815f3b750a8d4a7a04bf01e5e9f308097471" },
                { "nn-NO", "9e52e21b750e097a2f0cb158771693a9f4fd3bba411716023c92711d543ee7e68bd3f874175ad61278da2f9506cbcb457ae38f606edb5870a0837c9efa6c3222" },
                { "oc", "098106c53cf4462af0e1943782b06d82a273930f95caef7a309ab04f567ff58877b08fddfdc25c96de5508ed0b4b52c5fa4eb8d4104ee0c5ea5a49491377db59" },
                { "pa-IN", "19c23515c31cbf9ffae4ee7fe75600c7c5a842ad775d0cef196af41056b3f07a29baae877711e86c7f6c06272972e91d00b146094a25c8ead55ae65db385288d" },
                { "pl", "15fec96bdfa70fc35ebb09ff2719e64fcdd3f974f54a158b38129b7fe9399eff7aa613d7a58159f38bd4287cb58c36720ac3c991b3c47f6878b999595303ba18" },
                { "pt-BR", "f6ff413158088bb248b5bff07669ac7dbe67b8e4ec0b43fc18fa554ecf54fc77774889de086495b516233592560b6dc7a23a1c6632f0a8c1a8cf1fdb0c07aa33" },
                { "pt-PT", "81bab9a58004b216da79c44ded13e173d7059d1bcb9ee480976939a22338bf414775002ddfbb08af62971d85bfaf07b4f6f3b6764ab723ed00e2fdb950900feb" },
                { "rm", "e3898e8b1ba6bf424ddcb7b07e7e7500361d2d03978b348d5ae54b1f879f6748762927099a0a2cb7d61548fb50ca1589a8736bfd449f1d23c8421d7b8e164d17" },
                { "ro", "bb827de962821f3e9b744a1e3cff81d3bb8b3851285ad0ec0800dc8d9228d4a8a453c6a497eb2d631026d7e59cc894d3e545df706e86d44f4dc8faacfb055a6e" },
                { "ru", "ee5ff9e8884222b97aab40acecc51099d8528ac8436f4b6ba669226303a1a77a0a71d3ef1ae913b5e86c9b994210fa5d603e2364f304269f7c4112bc61599891" },
                { "sat", "6426946d279d48432af9bc584fa2c512e1de9f14903e79f82b54bbe8f7b9e9b682e3b37b5fcf9e8b202cc12c57fe4182a565852295a61592412dcf94632e1b84" },
                { "sc", "7419f15ad5161518d108fb038ef2f06e657c1e5ea77588f5a7c56a824a1a57ad26c9984d275f520805366f56e389e85a0abf01f5305cb40ec4464d3adb0b8eb7" },
                { "sco", "784f7c9f2c6ce8c7275493c8bb429855588f8106c685bd209b568f04297fb532a6780bc073d821dd842ce154fb98db455c9cd0bbecb80a8c2fe3e671f83da161" },
                { "si", "9b45cebed4bf40f37f692465bd15b573754edf4a8e739de8de13736c1b46b24d07779f260a806eb63e93dc924f7821667d18b07ab77cdee79d5167799eef7633" },
                { "sk", "e78f788dda53176020f2405a6780c661abc4d741aad77fab3e1fa84490ed01b775497b4c4ad57c64aec7b4d620e0edfa74e354f5562a93d59c52d68c9db0a533" },
                { "skr", "13f0dc08a625461ba0f3f52cc23ebfdd0dfe83939c23c4e988330e657363d7279dc0c2da2d306ad07690998b7b29343ec29fd4c8c52ae813797103571d735dd3" },
                { "sl", "77effe426db44786da66d886fa61c5f72363fb05e9a58722eebad2b5b1e7cd51b2c28e6078e45e7ec5b0a2557ed4b0ccb9f255077a4b782295cbde5316eb9c71" },
                { "son", "77abd0d0dbfa965c0e6df89c31fd847b6b56614df7fe15679ccac66e445028a338e1ba0669ddc7b96a128f426dcd80d6258fb25fb19773af5d5295195c483349" },
                { "sq", "cbfc30021efe7f1f38dcd8b4d00f235b1fdb2e8ded59b4b3cec776f4665e0dee5a6fab41328d9d0c736ad1caafd90805cddee27d6158398162f89b5131caa258" },
                { "sr", "4e9260302a0dc9bb5d624cc957733421a44234553cc6466fb148990abecbbc54dd65192ca4921a04990dff49564729a79d64931ee16f80dc9ea67346734c1b3a" },
                { "sv-SE", "a4329f0517468c3ad4b266be354ba7b0838c16b00e521ab580c5725c9b7afc80b14102e31f7706633b8ed9cb11f12158f63c62ceeafb1f80ebefceeda411b6c0" },
                { "szl", "00ea69f813115577f2d093f084d49478098295150679aa4f7feb8dc0f434ed1794ad1d5d0f21b3d1228238486161d397e93146136b245a9b26e6f68a741e232f" },
                { "ta", "5b5c49183fa08be62a1065c76882869fe26546562527286bf0ba0b26a4a8fec4d24c2dfd72c57545baa6bc5a137091a15e24426d5977852bce82da67b2d6627c" },
                { "te", "63364f6526d46b1d09cdf8b58cf5fecf0548b7ce104e0b22a93ab8aa4630b8c6a02da93b67822d4513f52981d89f6a2b306675c60d26025a5f160c7aa6f7fec5" },
                { "tg", "b556979506fb28e2c4bccab407298475f7015bbda9edddd23f6a47d777f1167bf213a9275469196d94272e0ba7a6936c3e360d2e880e5c11234297510ac56f88" },
                { "th", "366c7674cd1ff6b71fe01bb6d3f038f0942278a7bf4e66819a45b499418e4148c730c4c66ff80c6c453d8b257584a6c9c5257e13b49206310f7078b1c20f41ae" },
                { "tl", "133b5cb1e85be0f0c43cb781f5dacfd37c74a6b4fface0d6e97288aff8750a23ff6265bd94ba34427f69ec0a9498ad379921bbbff9b10b5eec2ca9d1dc6b3fd4" },
                { "tr", "cec7a40eafdcc5bc0cf9cb3bc204d08b38e41324004a2b1040673843e918761a3dbe2fb1eeb79f1fd7e995c0152dd0a6301b7df75b059ae4c68c93c505d2ec00" },
                { "trs", "7934ea0255f92f1ad6b33b9ed5648a45e8486af86d8fdda86c3ffeeb2db2e2d8fce552283e62ff035521ce526385ee680d4763ff698ee1b58d6b16b5b52dae98" },
                { "uk", "555f424d7db2f5df81e5ab2e2236239b85c1e37b42d8fa669d020041a66ae6509f80da46b2e67e37236aa95d5ca465aeb031e1a131311c777faab0915093c293" },
                { "ur", "39d771d4a476d2c22bde58c3c33eab89881250c34fb2d688f7262f4d36fbe06f2e26caf83e2aa3832d704c97a706753439e5660f3b65d97aab56c3b06692b7f9" },
                { "uz", "469ce2011e07600b0eaed3f72c5815b5d7c3b7afceccf2056abd5272d3a8aec336c8e844ecfd52935dbbafd4209f2ee0066b4bb53488474f4eedec2c93f97366" },
                { "vi", "7b307a52fa9e40b9a61da2ee5864c9097b9296d4bd7d4d503029b216facc624705e565a95d8e572ec2bc6af2b3abc7abeb882833fd39a77f996e3ee502c44917" },
                { "xh", "89ecf93a4798aeaf069b9a37713307159849695f59bbebf87f89fb8b2877470190feaf9d5cea6bd000f48a2a4cd722be78a24039c9c39c83307b54ccea9efd70" },
                { "zh-CN", "ced6618f5a2a340450a835ee0baa2b12d6ebe382289f0c8c7813f6eb06b06570f1739ec04f20661194598f0d8996f5e9d5915b9cc381f7bc642d17c8ab78af42" },
                { "zh-TW", "1dde05cbf06964889e625fc1c4491e55ae665823ba794774922cfdc5237b237e2ecf9f9530b763ac9d5e4c648664eeeea1a2b978fc697c10efab8a733e6991e2" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/129.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "5baaa0912d6512ef9ffb7b2aaaeda2a19e995fc9ef50fa21379677f0fe4e218e7dbcf03018a0ccde155a13100b12d81ba52abb46bcf9a898f7b5d57aa25684c3" },
                { "af", "a2af701fe8d917d57428d3dfd5cd42d55f522711eed3bc53061abbe5c588779b9a3e23f3846870b918296ee6f9b588c6c7b00a7fcffd910f13c7329818bb5734" },
                { "an", "1725384df95b0d40f63e70c7f68eea70c139cc7d2cf9e314c840af23c57124e2eaba2783d04972302bc23ae9e19fd13bd8c81bf526ffb4a7dceab740c2b8bdc6" },
                { "ar", "6fedd83ccc7e9218c7348800a6b3ad7fea883fbadb883fe0d10d93fdfc08ee5e9ec64f62ea8905ead09ab7651f935d0cc2caecd4565f2d0d51a8a9b7850a19f2" },
                { "ast", "d3eb120ed7679108de55aed232916debe0c0ea8f7943e0cd512d4faf6dd5b0dc9152e80303d4a003fa523aceb52d37603c19941c80a0ed71962ec02312f5236c" },
                { "az", "423fe29605247c7bce703714b9382f46e67650b39d7dc501d82ef91a53475233d68517bff9d9c58d7ac24ceefacdca0d52ea055a7b3c9cfbf725c9e64a58bce0" },
                { "be", "b6c06a479915efa208c8097f1519e432e9d4eec1120c2b95a87be142c5167ad626ccbf812533b16f8a42a295b90b89a23032cb493dad3bd164d0b885dfff799d" },
                { "bg", "55578a3a978036e29e10eef944b6aaa09b509a0a846c1bd2268d4160d4310535ef035386fb4175f6bb8009d8a950401e755933ac870917df92bbacb0f3f77b81" },
                { "bn", "5411d5986f240b2c3b5aa84d9edb7384c24b8e6a9e7b1c901e1b73536b905aad6142e7004a5267c0314681f7d45e4fb24fa831ddbfd974695ccd82dbb9cdc026" },
                { "br", "2e5cb54b9a4150be4c62bb0028d869b2bc2764ead260680729365e1c7fd3113b706b379235182207d79d538b5a5ddfae54caa890c0fcfb21179f200e0c8ad01f" },
                { "bs", "c20ba7f05d7e2f5865140715cfa63328f0a24868ad0c6752d71ba0b5b5e4c81dbf01fb5252638878787413f799c14cb1177f06420a0ed548a05e9eaf9edfeac3" },
                { "ca", "f1435faecbd66b923e2be8ffaef455ee403c9f1e4f0052cede39d97aa5dc700cef2d8244643e0ec37f210d6c5c73f6fdb92b16a57c89c25d60aa5e550d41a33d" },
                { "cak", "ab797ed5580706f66c27747f087e6f6f7475054205ff005b15fcd79d4802dbee52d56af23d4df487c6610e4060542130ad736671da69a0275c38ae3238c44153" },
                { "cs", "16a2b5b21f2ede0299ece3004441035054f516e8ca7f99d940d1b241d6736abfc291858bc63b1cf3f63a2444a4984c3c12684c4321db3d959218473f1be38ee3" },
                { "cy", "3b5d6bec7eb0af02f92489c82fd383aed3407450d118170729481c27ce93b197d8c4751d338fdd5358594648cd59210eccbe1f88f36d269abf6b8b83d4085ea2" },
                { "da", "630b8065277e6d183860c50fd38feca80f4655b48f40de4bbd004d337dbe95caf1ab607fefb26f1d7caab11bea4eb46ab5e83c1379679ec8adc2c7b09c77fbfe" },
                { "de", "53e07c2133c18f5ba365b88dc6364570b01787df17a694d9aa600278b54de673b3ca726c3fe121c9176892fa28c19c9299054a77da4ea566fe2bae718770ff7b" },
                { "dsb", "a35cfab76eb36ff27cca837f431b840a9daaf660b3851f99fc4b7f1dd26b4b1ab4c494e1b327d92a66776e82187d1da0e97804158f95c441a4518dfedb1c9a44" },
                { "el", "54f18462c01730395a70fca448f53412966385e85bb4546228371142e6746cfb167b758571fdb34c265525e4a5d7ec3509800c1ac75b50b3dfb07c4c295df225" },
                { "en-CA", "ce2d99881d4668efc658657e01c147b2e7da399d8594c98b715afec079ec318d5a004708b10f687af150d7bff503bd0f0f5522eeeb8ed4460347a89f1c8535bd" },
                { "en-GB", "f60df1d2991a09400d671bf4905e9482b7b1a3c7cc87ef3904e3730a729ffdce649895f15c2f9426e0a6df2bcd7eaa3f72eeda06038dd293236a44ae87413f55" },
                { "en-US", "b44891c69246d01d1425bbdf94f2dd511ce79d60494d3175f5a06692f2cc1ed0581fb9aee9ed0780423dc73b3565b4ab54ae359e5dce74036203c13cfc18a5dd" },
                { "eo", "05e595e5a9a6e94ea99b861036503b42717aa98a719cafa781019697c4f659a9acca9fcbc5a9ba0b644d60b231ecf5ff77ecc3aaff4a938ac10b47421763da71" },
                { "es-AR", "e1088a5f427142b563726a43c002d1d4e6dafc584d72ba4f26bd6e2ff0aed98d8b88d8c968291b0dcf790484c4eae3c58cdcb0c6ba49e04ac931ec49ffd634ce" },
                { "es-CL", "0db2e512f42c3a1580acf8abb1e84fd751ba989ad4b66653874c555ba913383c0a72eecb95dca8e5a38969bbf491ff57ea95bae040a0bbf4fe8eb5b73d6dbba5" },
                { "es-ES", "ed81a7bbfa6e8f7999330a16fe2d9972bcfe87cf5243bcc472d63a86591eef1be25c770c3715bb9c086c718c1ecd692bcdaea8b26b9745be63c1db469678967e" },
                { "es-MX", "06129487a5815de209a5ec0f736428630d632594b34dfa38f7232a5cf3b6fbef25e4daf6675c0c285ff338f6a12ec529d63792299d592d69578a2d3955cffd97" },
                { "et", "8f517882d1da19f449445ec119df4e3ee23e09df74a8df44a5980f20372822fd4b5d29fa7ab19c9c72bc3483ac7d27def1bbffb79112d317c9157882dcc0bd6d" },
                { "eu", "943303858fc90bf7213243ff92af140a2d88802e51102fa05008dbbc03bd529cecb6df6e53d4cae844edcc0f9a2da05d7e6e6362f107dd4bb416ebfe033f8e92" },
                { "fa", "37f281cdef08a9e782e72ce660b31a65a6a553a99b5b56c9d056888b4357471e419be18517df170353e61d9f46d77322495cc8226af46b15602db8b665be407b" },
                { "ff", "38d8542fabb378f4ae31d6cdb318c5300e73acd48b3c19461037369d8ef2ada80433dc760a7648e95e0804c19bf9f000bff79295dff6df8a0220f629bde900b8" },
                { "fi", "80567a4985b51bfb083483d4f45337abc4fa1a36d8500453ffb329d75c5ce28ec417e39303d66f63cfbdaecd348dc7ba7fd7ebee8469a3f7ee580930ab417425" },
                { "fr", "e2c77497d6159ee35c56bceaa21a358262ce870d4fca363ea2d8dc51929c5de41141075758618a4031f9f05cfb916f860d650d0e15430e2e1b2827ab96794315" },
                { "fur", "1540298905c9b1223c30feb7df3f4bfe5d71ce64014475fe8d27637a257930cb23ee3191df3157e7ecc6d63de596cdbb7e35ded8abfadd28fa64a7cb85573290" },
                { "fy-NL", "8419f525c5d82b70bafeb36baeee472d73ad6f73fdeb0f530ebdb347f1514c67a0b0d06011b116eff6b75c974aae1431b2475a29a6ba8627eef18aa673462621" },
                { "ga-IE", "7f3b5d18a68a765c1df1e8db50a3dd092a139cb4a41cea1a009cb3ef9e6f6be247f75e49a89210ac841761266c2ecea2ee92fff394ce4506f2b1f99b435115c8" },
                { "gd", "f4eabd2bf0c055ee2a5ce9fbebbc3e447c4e46bf7c0985fb397f1673bb2640c497e43f3ca8c73ecb446f6f084d89a77df408c303d11f7321b4af79ee0a85df6b" },
                { "gl", "d4715cbf6cd7ae5db806401937c75980d20bccabc60210f1ca95f7a02cb896e4914beaeae1b3142f6a94488445f712fc5cbb847662cd4ac0951e962a7b7970b6" },
                { "gn", "04a3a45563243f776a8659428bc01e284d0ae1a78b11aa7c9812201287262165ab0f7ee7e8dfa70bcc406cf45ab5af990da81507aff4115669fa3e0ac1b3bf6d" },
                { "gu-IN", "8c4d289b2871408bf4b279a8863da3b0405d20b310c875d525e7b633a457fe09b139bfeade4f54dad31b5072ad5df57b563e49ceb5c6309f430905bfc00bd733" },
                { "he", "4fbf11a876b2982f94fc31f1ba89b7bc3581e913c42376b6db228b3a8b1d478f76f933ca223386fb15a9d9ea29c25edef4cc93a527ce543bb0aaa7780fba347c" },
                { "hi-IN", "cef711788efde7825587272bda1189de6c96a97f9458295873759076ded2a4ddeda2a600b4847f1a8e22387d0799d0b9b9a68834dcb74ef0031407cbc24a98c8" },
                { "hr", "436fd6757f3e1cd44e2a017aa5dea910de132fbc698d52e5b0442e150740a1d5d27b7dbe7ed04b6a0c10a0f53d64101a036724aa75660c646aa3f9cd526bbb9b" },
                { "hsb", "e911b6d9aac27de35b537e095010d6874f49995abafbe9552c18defd46374679c2218fd65751e4e633b20ab182f4f466f4d4032ce5dec232cdcb2e5367a84fa0" },
                { "hu", "0297b83df5f56eb1f0db1ae9a45d13ba97662e211396eb6325df1b716668cb4d219862d08f077cc16f34417c3279cd2d84bc132bfac4dbf2dfd8321051ceb168" },
                { "hy-AM", "a272de374abb7049659eefe5491b446e81b672830757d02ded40085d3c74c84cbd6f9b32c65a07c5a120e3b32882791f46443c5c9891bda66082384e44fceaf9" },
                { "ia", "d245fb5f6f785124c19c52bc4f58ff0474e29784773bff7ebcb65ff8f50a2db6e5ab0f22ea986dbeaa3ada35e9b2eeda3b17e9cbf909533eb336539f392a4d07" },
                { "id", "160a264135451f84a75f0dc2f273a20ab18eaab5fa5f7ea0044ab5e9b43846e3f946262acf8579462547311ab93c0497a0ac505148c8f4eb02117dfbfa085e12" },
                { "is", "cd61785a41160aea4402032f96c564329c80e12ce71e9d2a4db6160b70072cfa53dd49a20d3c1b386acca47c0f8d0d71a311d21651cc6d34b4ebc5a96ef365ea" },
                { "it", "d65f1de59fee02647cdca3766b742c11fd8bd9c1e0ad2d1595414084cbb54deae39eabe0626092bbad776c27ad45a2a6d589e94ddd9a69888fd25462dca8311a" },
                { "ja", "61e8fc7a22cf59a025f807bb532d2ae54addd45a7564acc1dd5d82e3d5b603cb7680d4d38fad4ee6ebab74539c6ea2d1e0b7c8995d42ae91e03262a87b788cf6" },
                { "ka", "b78f16d8ebf378f48c59a80617b6363eab3232ae78e2fe00f36cb5ba41abb5bf4ca72f925a58ba844c714b8dc07f3921ad5c394831af8c02ad4839faada5c99b" },
                { "kab", "6dc2eda57a0c304c70c18c5bea9f0c02b2127535ad55f5a44c87999e4124c4f71f710b04ad5eb28599289dd64f3c84542bfb0e2ae631d6ce768ad0399e6cd7b6" },
                { "kk", "9fc98d802f9bdcfe671e23b17651d67ab04f34756da4050aa22831efc45a7c2f179746229ce0210423c62be0ba6d2b8a0f6d49fed64a3819431f4958c387d48e" },
                { "km", "35fb11ce3a57f6870e98ed3f05a7316f8ad1c19e9e7569e1717c745ad43a04cf8fd462332c81af745792090c3b59cec3d7be7c91c938e1a9c6236c13c41f3e4d" },
                { "kn", "b0f7914e7ad2b8d8be433e468961bd04f94893112129cb346b0fbf0b5f11988137afcefbb4d1bead8b1051a69ce201f8a8aef6952bed45d60feed0aa88c2ad0d" },
                { "ko", "0ed98123207e51cb4167d763d4bbb92af7138ce7bc690e728892d90246bd690533352d6694ad6608b8a020651f31d3bd52ed2c33c6ac463fc0f71522a9cb82b1" },
                { "lij", "3b6e66caf614fabd33afb62cae5466327370c47e52f299968af0ac463133d843cf5af4d40edae41d84af69cba9337049df5e119fb634a143c8eb18edd97af154" },
                { "lt", "bcd77d44b46b453c23c19de7862ddc90555a1a0ba9e958f3271e25b554b59515b19c8595b6b0268af2c9e7502fce10520c0cb7d13e4338dea29e8366a7118d57" },
                { "lv", "33c60bba1e39a3029778929ec4ea83825d95664016163f6d6f1d2104661bb5f4ffbffb3405bb8c9601017231bd73ec5f656e449041240c293fa8144f0fa190f3" },
                { "mk", "4d33de3ae3b54cd81945958d4446fba4b78423e1b70c9425161e9690ab665fe8696a5d9f49a08231e5182f746ede2af113350c1eb47142c1a2e46fa1700126a8" },
                { "mr", "0a1576dd1d6a048d4f7758779343ec402a4da5074954778b2173b3294324559af14b4c3be2b0720628c92f244431285827c0ce781027e9f12490c0962128b66e" },
                { "ms", "a31660d804bfc88f53dae325de1677d34c65b5a1c9a70d4d0879eee8ba7b9451d6fc1242da3901c3a07ad4f92f69c409cf54b18f8c7f8401458eb2b0cf2ae10f" },
                { "my", "a75afd677638b7f5b7a69b6538501450037366be4e10cbf4bebe369f0364c8f6ebd6e26008caf4a86fa06de8f872fa74d96b44d80478619f8714a3f26e6954a2" },
                { "nb-NO", "15b55672aa1ebc38c71684777b3db8549e5bede8477e25c06d7a2d28c3cd751f0e28e99ed734a88112ad9461a4569a7a200c1d86dd2706b3b5990d71fbc299a4" },
                { "ne-NP", "3f3a62b2020eed650531c4521fc9f23815dd26aee8c4ee392acca844812413d9fdda4c44ddf6be35b1e69ac58294f9437061fc46d1d4859a6d27f270d908fae2" },
                { "nl", "35d1d337e2013b4c6e96b26c04c835c3fa65474965b4e97110a831315b7448c6062e3549ebf7143c87f7f74f45844e7792ff9078ec1c87a51e32da7f05e1fceb" },
                { "nn-NO", "90132d186631f343b0bac6e6449f11e571e38d7f09c1d29d8c34bc587cf7f894f8bfdc406567301061bfd72086ffacec0a60568541e2ac234ddae37e6c63c5e6" },
                { "oc", "e2ff7aed17b9be758f9a7d89a2a6e8b0ed14dfa397ab9c15775b819d4a9a4a1916472a9617052da463e3adc55df7892e1810e696f07604e8bbf5318249bf27fb" },
                { "pa-IN", "81df88f48ca4324e13fcf3577cb248112d43dc0c11163eb6bf17231828b1a8d0ef300de2c2877a5812fd7c3232244d8b563be7fdabb17037e2042a4b844ff1af" },
                { "pl", "1f2ab44bfdb7c8e7e96d21695acaa1d4a311efa2c155095ef9c95a4d0276b20dbba45b2e4118fc804df95f1987f7858f32bdc6039f86d07bc70101b99350608c" },
                { "pt-BR", "459db484c8e0f2d45d0a1a6ae6676c576608fc5dead380929ba42643ece6fee7003b2e2a5cbb9e09d63d0aff3994d0d8624f68118ecd35cb977d7025660de72c" },
                { "pt-PT", "ece1771401f8035fbd354c968c4eed50073aaebc88a34be51ea68f9e0150e8ba5e55457f0e1cb848058da2a4525205a294f045de3c8fcf11898d17125ef537fa" },
                { "rm", "e415bb35cce9617011f4801793333cfb0eabeb8790945afdb858f2d7f130d99e431ab92279de0f445cef2503252a55f8536702ac05f48daac43df07fd02414e0" },
                { "ro", "e4960d3217312becd7f269afd2ffebba6d4a4da3e7c58ab84365c9044bbaa767fe9cd5f8d7c86d073879476417c06083313df2ca3a7031dc617e5b8f8a584b61" },
                { "ru", "6539f6da4b09e62177f78c68f13dbacbfac574a112c8ff4ba8dc33ba6754548f85b55ed30be97089abd9e6a3608a1f6dfa8a003675e31f0ad2006ddcba4e309e" },
                { "sat", "f643f45f6266592577a918ab720e46845369a3f250ded0e38293dba4d0f7faccb63fe015f17820b093f48acaf719650b1fd17e2b8178031802943588068a5bfc" },
                { "sc", "6e21afaf380fcc5f76da339ff741d1a955f5e9f78f06b16f0a174c20f26707ebc20e7a5c741d77abcfe40cb3c947f0594906d6cb0bbc6e8267294749de8203b4" },
                { "sco", "ca8ff49a8f29ae799f8ac1ce3d0a045b73cbb0449f5f14071457e2240737d08e3b6c90f1057d09745a09d8706ee46cb7c8c431bc446bd1daa24585e4d36dafd5" },
                { "si", "47d5cd0d7bba7c2e5c808925d70e1b731977b40aa6aac8cb0876f76b88b7956724a93527b8a0bdd173bb071f9e7119127a78778e2fd7ebe1a46124e9ae33cfce" },
                { "sk", "ad71d7ee8be30692a4ac9b2978ffafac6568df155031a9ef2d3025b00d189611e46a7b1ae410e404cb9032b2726f20dd279e05bcd8bd450acdbb809bb8f17e9a" },
                { "skr", "fc0f37d18d35fdef0bc2edc27c1ef07001d0d4fd10191b017ab8813c49088fd1c737953183b9b0675d62188a6887622a347e6a44a44a1e4fdf7643dbd613782e" },
                { "sl", "7814cc3c9e7f5f6324a19cc06c45cf9787b4d9a9a03fdd704f2e08ddaa20d19e91af45d05f54e6847a4a1894af4eb27f4de8761b86fc995fa6b0b881e7d3f9b1" },
                { "son", "2703fa6fa2b7d53590ea955a75a039b500a6bf412a0f7455e2cb26b4d069ae33b3c69065edac08b4dc693ecba8878149021eefcf6d1fe13469e1f4dd7e6f59fc" },
                { "sq", "b6bd06ec5d04665da140dea2e194317161eb07443782dda9e8a520252d8b422e9ba7ce9f08c6d9ae5094b10367fc1fa0c3f57844245c2923f7eeb033e7cd913b" },
                { "sr", "66efa1ddac6d251aeb53c99817abee1109d7beeb09ec5c3d6c3b8159e5adb486283aaa264ee67e93f383cfb3ac879db188aca3945b590917158aead68bdd78b6" },
                { "sv-SE", "084fbac7cc37886f4c794b1a8e90f1bc20f53d569bb046e8709fcb56cf65485fb7b38c850a4ed48a503ca0c4f1875de43f0fd5edfef555ef92a913771b39eb61" },
                { "szl", "30f7e39c677ddcde0ccefd8fec528d741f15591b2e6a6088e4c41560710f464b7503cc2eac19045eb9992971995ddfda9c4e9bf2aa5077d9be77f9e19e967600" },
                { "ta", "c0df11786fd47ac335fa361e3da61c86c50dff37b4a7cafbb8e300191dd30d9263a667bcf5de27dd33eacead7f1b578fdc158933a3bd6c45dcee68ed8834cb33" },
                { "te", "ff9cee5701698e7624baf16c7e78144ed6ef63f1845de20482518ed5adb734791c62420989eb04d2ab579adb1e5124c4ae77166a5f3b5c97d6b79e75d2432969" },
                { "tg", "9e3c917d2ab514b777ed39b703c20492cc58ebc7af2f023ff144aace4f74e1dedc34da34de1f8905b7de67aaa16837c7e900dcdaaf1e5fb61dffed8fa5d72386" },
                { "th", "7e57240b5adad73b3b4b0f453006842f58c287a5edaae6dbde395da10d4c92f54e1501b308d59f7871ef7f91c9a6bec9ae540eae149da22328ee4e98d5353039" },
                { "tl", "71d1c958a2c788876df218e4785f0c0ade76c596b2ca49223759fe6c99e2db2ce27566c365694a8353d10d56b7afaa30ad21b2484d39d0803be4b2b9e7a83b7f" },
                { "tr", "a4357d4e1988423712626b14c91bcd97e5a9e385c537c9a5efbfb2485d286dde1ec7aedbbd53cee6e4beda7473b7aa7ea087c33c557dc4b93919cd4a35307791" },
                { "trs", "c0e27619472fec860c585fb5d515e658748ec85084312ac8a9e1cb5c42827187016dd5d38a9bddebcf6736f194475b2462b88f2cff2348307066247786d08e20" },
                { "uk", "bdbdad8cec6c1edae05a4c19e9de972dd2c8924e4f7d818a74efbc21d9c8d2372123c99082f8ed1390020f14c54b2cf7cfd058118bd541eb534e4204ea390c89" },
                { "ur", "25f57b2a5a05ed6e7c952c0b09664557469ac4a689ac7a1e07fd7c6c4b8a09c59b83296127af0d5644a1cf144a4d07488b530950752cccce41cdb7a328999277" },
                { "uz", "a90cbc7b2c56f25d92b9fc5f1a3bb06c089ef6c7a5c6232ae2a76775b9ec090e3c6c6dcf7c3f27d741dd0f1b13f901e950b88c2af84ebb28a4cc1a47cda89a07" },
                { "vi", "2925a1ae0119a5ba7f2ab58425152653c17184abce3da4ec49b955d0a6c96ae02a811ae90890474233e4c97e6118a564d2a6182f71313986bae6ee71e0c82030" },
                { "xh", "4e3723738f9abaac81d7ab4f693f1e9c3296ee6578f44763f4eeb263e2f1b98933f838d42619e832c51997f0e835a9d5bdda39bdb92f14bdf3b396ae65965cc9" },
                { "zh-CN", "6a26c2d31156db47541e3591efa4b4491149f363379007480e88e69764e0ac9274f583fa8b5cc813896d4cf2b5ac1831a933d3eb39a34356eda269e753a226d2" },
                { "zh-TW", "92d6e9918bb8419d06bd6af7bf55e72e8b82f71457c77b94861bfeac36361372cd23f35c8bfa9a1d48deeaaedcbd4b5c7272e3d1ecdfbad204b943e1c2346fe4" }
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
            const string knownVersion = "129.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
