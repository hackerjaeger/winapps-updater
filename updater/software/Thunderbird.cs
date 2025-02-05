﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// currently known newest version
        /// </summary>
        private const string knownVersion = "128.7.0";


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.TryGetValue(languageCode, out checksum32Bit) || !d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.7.0esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "b5d1753e1e6a7e78ffb12bc0ee4f2c10915d140b8317c997f326e7dcff6b5a7feffa6caabfab165caf47d532e3ab0f9ded5d6ebd3f6e56b24cd7c19e2a53a231" },
                { "ar", "6ff2164e52fcdc8e8e64c55e41b608e99605529de5b4f95901aa2b89b89e9f1cca4e58f452abc301030a1e5f80548bd322b5d159f238d4217434edba2a53af89" },
                { "ast", "5fb9d3a2ddb28a6ff8508b7cf51cfa1e81c8e835d375f9233ae0ddf7c8c8545ee5b7036c6d61ed132be63bb9702ffb4d8113c237a3e855772566f885f059ace2" },
                { "be", "3c28e5b3eaede1943c04ff983e8647239dd7d0ca31954dcbff4e9b036f7a7dfe4ecead724b292cc0b3521261e84e78798b959860895e4a7190f4fae336ab2eff" },
                { "bg", "2b9843f0f2cbd469ea8090a3c849c6b15a3699967daa9fb00cdf110bf51744aa3cc21fe0d8e0c8d3da3af2e56c6295c79e6c360acdafe5dc883ddd677f2ce820" },
                { "br", "22979049c13fc47c8e04e84505e941db352da5a4a2f395a6c5afdc3d82ad76bf19eee0f6dffef37f7296fd0c50f23a6d78ca2ef4144b10b404f53a2e24ee23f8" },
                { "ca", "4fbcd0e84861be1f3a3e9b39933287060ec294a13305cf6859782a4c5a7fb55169376706ff37b567b3d48f2b4a5ee80424e3286f5e2dd57ae92fc15aff2d9af6" },
                { "cak", "d9e2269736b0435e9e5572beb22f41eaaf9d36e7dc865d6d52daa994a5a3c6efeac9103a14ee913f7a8b29d4b619c1336d46dcaf6bc2695c9065c28b8f4037fd" },
                { "cs", "c97c14fbd8247818873f2e9b791ac678e509d9728ad90ee2c7cb4c3d2cdc9a2af688a3fbce6ef8c70138a2bd163bb7838ff8442fbabe49fe686ad4d10c6dfa32" },
                { "cy", "1464a3b5a1c0f89a0ccaba471f2c1ce178e4b541e9441c572ed9482a98173534431fd738d4e970e9fb17f42e0ff5ba297b629a2f3236803f1cd591aacd6a14b8" },
                { "da", "ac598c4ce91139626168e99cd3c899d5a690c1b7a537553705afc4dd4ae30e298d622d27a0340955aaad68624846fe5422f5e0cc8279eca6bccedd23eff825d8" },
                { "de", "51f8ce84897c618cc1a55eeb101a9a1898f5f4eed4eb7cf9e4256fa5dcd5bf5ea3af40921a4c345240fa7e3fae3616010d62b013fc56d26affed66346c743195" },
                { "dsb", "9a09b10c7f06bead080b7edd337176ea2a3c1f5cc77fb87e0cdefe2185886f0230e13cf179fce6ca65cecea9c37a1fbbce240da10bf021375663a205fb7eaa8f" },
                { "el", "5384957a86f0ff822a9ca5a118746fc512ed80065c9a4c4c65064ef3b1c589ad610424f8c798d3303a5a81bd238a9766f01f8ad8616170dbf340b8eca1f82b5f" },
                { "en-CA", "f925ae8e4fa8e4de653f3517696b31c22a248385b2b4d81ea11ddeba48ee0f505dc7c345361575f1789e7117f0d9375ea11e072500e5654e2e2c924e2fca99cb" },
                { "en-GB", "5fc939f7ddea40cdb4a74c0e285ef2f7c6807a1052e3ed6d99f61f774afd6a3611f994c705ba816f7fb5f7702eb54ca59772f8b022e55cc28d069a92f1e77796" },
                { "en-US", "2e131546027cd0b6df0bf71907ca628fd2986231953f666457eb15c0d3351eea80fb95e204896d82d4c064f187d671b1c3820d38f191c5690122ea7790bc6117" },
                { "es-AR", "459e820830168e6db14f869a5133a2a7ee546c151904bad663f3c1de4c506597c6cbead5b96655ec4c75a50f11e52f9ce1fc50ced703b77f5c7f4ae9e124dc30" },
                { "es-ES", "fa5daebb284b973da59910905640bb7efffa7634509cd1c936c59f5a487166249d7eada16fcd5c15f5f0d74e523b572c7a03c237e20d47e6f18ddddb9afe8f13" },
                { "es-MX", "44dd9250bb6dc59c008bd512b50d819a805f68e0ae258db3116e808899051d10b43f1de1efb9419ff989cae0804345ffb4ec2023b5a759f6ec84180fb58db5f2" },
                { "et", "bcf2d22335df45fbd4131e6fc6c627a79281e54f874c6b93f92b4ffd6932ab5cb434f86cc5e0439b5f2093e70cba98fdc2b5e77c5f1504ac21f248ab7fbe80a3" },
                { "eu", "22c7480a16b472be5dbd88b50124400cd05a173132a27dc807d398839ec1c1bdeb506d0459cf2288a6b69a725246918e389c4a572a5712024bf5ba0e314b98b8" },
                { "fi", "4682bcdf73bde22f693bfcbfbf8a97d650302ffca620042f314acb910600ce016d754e32fe12c2c56c5e06b5d242f90307ffa3a5cc16f662e6fa7e290ba8cd41" },
                { "fr", "459229e4bc0925118b962292a9cb56f7034b945c5b884923cf2413c6b304d78c6925e8d4e0800cb42270e8c88b80b1eba42a8add59d3ec8150b9517a3dd603c6" },
                { "fy-NL", "c2aec548aed0f142bc53c3009e7fce8008c91ec0b07c28dcd01ad4835863029647eced9022b2cc091bba556d04cb3170ee42c36cf9bd13acb76d58fa1e405fdc" },
                { "ga-IE", "af6be093fca245196bafbb54d56a0a4744e0ce5b72827e7f6ba26ad9c8eaadd90326ec9c91a54e23c49c73222a5c444222452a9ae8fb20b331d8da888abdedfa" },
                { "gd", "a5d5262487826503418edd579f42298fef99aebc50276132e5bbc7b933654761a4f59b8aff5663348d485444b4fa7c41bb3bdf48beda9858890735c3aace2bd8" },
                { "gl", "e0557b9d091d9ae85e69749f31a6cec6b4cd1ecc62df1b1fa9daac78e5bd01ee7ebdbe1b6a972693e1c1734c9cff10056a98d72a3f6114fceacddd69f27e6078" },
                { "he", "86b21394d13367fd2069cb8f39bdbd8362aa4bb516ca83743a21d953bdf267f63d30b69857e58cfc8c913838a202368a22be5401b64d932097410c0c87dd5271" },
                { "hr", "49d96a55788435c55d3cfda0e457ca72af24c7b4662d52ded77917aa4d79ecef613fbc161bd0aad3554baea4cf86c033d52dec61c624ccf186e6db50dcaf08f9" },
                { "hsb", "c721ccc00de36448d46b2ea20719ee70f2f8e5d20d90267563d5d8bc147b733abae7c44d7d3343323e23aec0ac5c8a621a9ae4a42ed22c256623468e15991d11" },
                { "hu", "8bc069cd4f105646c21bfd37c59a1479f32a69963942f1687cab9998f45650b4d9f0c336a551fe545249bf2f7343eca95587345f57c1da4a70f9010a055bdeb8" },
                { "hy-AM", "766592ae13864ef1422269638a353968517792d17c62c8d0f9f888874a1bf8495cca8575f0636bcff5ad2c74f569cc087398af064b06e966bda05971374c9146" },
                { "id", "cf6fd96d40807f8cfcf799c2eda49513bb565c8cf3a684a32c4707d42f327ebfde893b1a52f79295b15f27efbccf98d01b2e0c8bfc4f55848528c5ab173aba11" },
                { "is", "d1d77d42bab1e0694ae87a014f011c7c84ee36985655b1372daa5c65c369551c08f1807030884483cd02984f979ed35862fd3a4c0f542b7367425150550d42fc" },
                { "it", "2fd0effadaee4c27dbba8d9082b7e2bebc7ce07c47f578ee61e9420ce7dc2b16f8eed1ada886aad55505f24bddc44c7d4bc75fd0cafdd67300ce5951cbc4b9d7" },
                { "ja", "203d2b3915b7345d248bf6381c6ae50897330eb526f66c4db295f9aef7536be42fde6c12ae9917cbda9a13f98260ea0c2607b798395f7221fb32df82e116d10e" },
                { "ka", "d60daa38649eb28ca64547999959ecd6331f6e4e59def8b8b52517e2ab932bc14bda31a8ce698d3003840b67307be46eca59d747b6f61cb4cd918642a8979e9e" },
                { "kab", "68cad236ec80d196b99f939add57f54ba3c5822a7a5bb8ab1336d1d67d0ec709831a9e3bd390c4dc24e23443d1c49ada5abba81d8324304036491538def121a7" },
                { "kk", "ff650ff88cc083536da4cb5284a6fe0e2482d687520c2a8b5531ecbecc248e2a197d6c4295ee56408f13d82cbf25e8844d66a353c05ead5cca9e65494ce3b9b9" },
                { "ko", "a4f5f55071417efbf55d70819264c3d30bf10407afdd1ae08b80c5975ba0f86bee31aab90bf76a92258eb8b3e788ac7735426eea41551d0781eae95c51906e08" },
                { "lt", "f3e600da48652bb68ab3e83971b2008d477d6de4b3f455466bb43161f4a92983e880edde4dd3325e796ba82915659ee2f64fef7a82707b2aa09dd54363b15457" },
                { "lv", "ac3c54202073e83555a894f4fd54404dd18a0baf5c722aa4d0e6185fe7a80f41f63b28f041f45c569eba090e896eccefc119b81d149fc343576d7a5e98ce3549" },
                { "ms", "bc347d0060dfc06a855d5ab2dde3ca7bb6bdf286fe34cf774d7da00716642370a099782e772bba6ecb0ae42d8d148ef3bbd3801c3de9e8f9168f69990b630f3c" },
                { "nb-NO", "e0fad119ff8e03f9e5b3e97a508be8639d4eda0faf878eb0548dc5c558458521cd4ef95428862d5c73b5fe2ebafc85bc8ddad60d81b2642d3782219be7a38bde" },
                { "nl", "d62406ec0b08bb96f13bc76aa1b80ef15db19a8c4a98588b1b9915da959cd86866b04683f84a5b5ddeb56568e234c3e666ba7c523a65030d0295767434081b9a" },
                { "nn-NO", "4854ac6770956e7dc55ddbd756c4cab30c9d33a35497b76bdc689b7d9c355642b3aaad1d511b30759a015595ace8fdc9633f00c691103e03d9dfd1c683cb95a1" },
                { "pa-IN", "c1c9b443a6dc13b69d45d6934fcacf08b150348967333ab7abf652d4761c4f018b4d4a52b6fc70a62805d223d81a0171969ab0b0781585f6ef4fdf55054ab3e0" },
                { "pl", "ba5245938c07b4bb427429385d65dafb97fa104e57a4e7638f317cc83b5755d83af3d23116a92f9737a35e246fab09e4c8e889c2fcaa211525eefcf0318099cb" },
                { "pt-BR", "91683bba27fde9356cad40853f3082ce45b82866184b9ab6519393a5bc34c4392aed4a5377349bdbbcfd9e9a2ae0f7419b2004a9f968bbd45e893d7353e7b837" },
                { "pt-PT", "ed7e2c0b89dfa1853f2b092b0d0f6f39307e5ef5dc8f5051406daec363a55da3cd337a09d8aa37bf3ec9e54dbf268e94f4268f72bf18eae3e184e7ee8771ac18" },
                { "rm", "ee3af7d308379e5d57a545205ce041dd0d762c027e116fbaec773707eb21a797e67d8f00e7a2b33df8aae55654625be8687d04da07e9c6afb7ab0c737cafaf9a" },
                { "ro", "80908d70c256b1feb68437360a7219b8e329a7cc8a962340f192f4f1dde3c98c57987507f68ee421df64bb82a7891516f71d98b35d3bb3dad4fe25c44c54913c" },
                { "ru", "b909655e125546241460d21dddab958a9ef1e8b587d717f5bf758a37e4ef51f91c1fb57d7afe17efdce42366075e56413a94973a51b7c538a9b5d7b9ebdaa543" },
                { "sk", "193b69404aa25da02d217673f577ff91b099cd4ca89c6d9423b30badc2b8390838b27012b723519b22ab8cf29ff4632139fd17aa705c95315485cf1c6fb00dd8" },
                { "sl", "365a3bdc06e328b71c668e399bb23ad8fd9ad48ca13335ec7d58642445b288c12e40ad621390ebaeaf97a57efbbfd21c6b2ebb50ba6fceaa9799eb3510eb8f65" },
                { "sq", "eab17b60b7cd20916d660d9fa6dc80672722f87de56354a57637fb5c1d575229ae9ca9ad746974ba71288db2d006860d2a3eb2204ef959606f145419ffb47c43" },
                { "sr", "368ae09e719ab80157de495456607de029a954715bda781288fed2ad442d49bea3b21961e37088b43162e0aae98ce6c0e15d3514fa0044a0d95ae8f161297461" },
                { "sv-SE", "11f3d1bf5bc8ccfb2a3ef10f2115bf2d716e4187d280e0a32d875738d1b65941910c85b1b2da436be0b52984f36985717f99b7698344624455dee1c06610b6c7" },
                { "th", "bc2e38a09bf3bd3db389716f7ae1810329e6b6b4927282d9c20e86d5301cd86138684cc718ff42668f31da2106951140a22c50d1c0cc25bca6da86d17f3899bf" },
                { "tr", "acbb0523ecdac9122c87409057a87bfea97d788703ad96f9de107992076211392032e800807c9a44908be4b9b758f6eb63846f0ca8a46f1118c5d10fb9c64ff9" },
                { "uk", "a6354a1b560c3a9d0c4b248dcf18e0722bd8b8daaf8f3fdece344cf8c69608e3c779db3bd5ad5dc3bc1d777e7d7eb08d5fa3b1831872b9258b83ae5d7ff276b6" },
                { "uz", "712ad531dadf56555aa490cd82a99e81245acadcb91d938bab6597a726c961afff5f2b8e257063704c8eeb6770ce27d80398ee935156778b5a856c36ef3b6044" },
                { "vi", "6574e53576ffaa3b13e25729f1fa701757aebb4a1de914a39c328c43b3ec2f077451472d2b7154c68a78ebdc213ddcc1de483eb573eb53f5bf1436b7357f1dd3" },
                { "zh-CN", "e4ae829e585d53e3a0eacd06eedfb625c50457ea623c360567b0775c9c7d40fc896a7f927a8ae7effa7356e91e9f33bbcb501437ba9c9bcb0361ed8a2a854c98" },
                { "zh-TW", "f306306f8731e658719c6d4695668d661394e5cb532401fcde39b1171eb85eec764944677b6fbb8ab09fbc68778238d5fe401090bfeca9d55a5a3afb2791f348" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.7.0esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "ba4b59fd0c006d360c85cdcbc1d626e64962e1e24c1567dbe171003b803452de4303c2ec7cd3f74a2b048006c19c1a701813390e8537501c3d596e65a85b24df" },
                { "ar", "2d6f2fc64bf2e3baf0bb9e080b74c70ca4bf035b8767b620ea6606f4be7b53549f4ebeea3a31120c4338e6057f41687033eaaac9a5b45148a62dd4417b51640c" },
                { "ast", "194ad65a6b791cb21a576031a9e546d6d418b6b15ad7c2aba01661463d26ee19702a6019df4aada08adc562e9c5857b07e90813f4f083e0322a172f6d9d1fa36" },
                { "be", "e9c4bcf5871af5dc9b1db69776f4cce59c6a8d05f079389ecd138be98adf2cf6baf6f86d4770ceff0dc02c8e81c643906b9b3a49fe8d09153e2d3ca06d43fd2d" },
                { "bg", "20df9db4f86ee9e092894bffbd1d771443569a4d1cd7b3201c5f7321a83fb3ed2a25efaf69835c34e77a1b59cfec83449ae6a75a60a8f45087d210dd5b0d5a40" },
                { "br", "0d197d55631d832e81a376f633e4ad744a51ea93a840c7e373cd26d1de2febef74c38c686d8a9dc91ae1f0ec4bbbf12a129417536c7dec341679c37945ddf0ea" },
                { "ca", "1bca79ade724bb1c0f30665a4aa5bd11ab3b73a43488cecde42bd96b5d3d77ad6810854d273455de70817c0201bb346da9e8f4881380fec46587d9884ba709d5" },
                { "cak", "f67b4a9b58982f2f88afb81cc12eccd83bb6a6270b5a3660a34eadac9631674593b987badc1d37fc5c144093cf1988e48a383748012c70fe13bc8fd5cf3b0270" },
                { "cs", "e7028255bbe50c7b18d0c5c98f1dc6a69a41787f7002a17dc5361a02204c4e5cb964e3f9bd574af05fc166f3b7dc3be2592aa19059b94275b07f72434aec346b" },
                { "cy", "fc5b3681410010417bbcef80575f0196a36aec3585d902ff9e680bdcc620f1ab51e4c8b0486e548d99d2713d2a0bdd8a84831cc2f778e025a45d93f7dcb10973" },
                { "da", "2b22efcad2b4472020052038c469c16ed97fd9ec43c591bd3abc0274a28598fe46db32367cd654641b3119549fd2dc7a216f29bb59e6962789275c5b2d32878a" },
                { "de", "e2eb437185534c87af2e3c80aaa151ad2368297d6680f1ad3b77e76e556c90cb10d83116b190b70229f773e591994e09b611fbde8ce5968ec1bc2066d9ce591f" },
                { "dsb", "919962c1334f7a56bde14319969d0a5c672cec8883351dcd739e8534a8c9d22f1a430f42b2fbc39e41956c6051342adf4ef3adc8e67759457d086e7556891b9c" },
                { "el", "ed3db835182b156e2394d3122b78e71a091252a6dbf9ea399c3f669724e6cf59ea851c18d7ab716d62c1755cd2f9035251dd8af21115ac72568060434bd42db2" },
                { "en-CA", "4fddfaa3776d5d42455a9acee3c6d777e35750f017a239cd8e5fa25433ba026afb05054912ceee559d4841e88fdb0e73fe4f7bac1f651ec2dfc7a2fc5246632b" },
                { "en-GB", "e034bd4edb3c6a8cc93450bb282f7f0dda85088b1d582e3628a249ff1e099a4b33b92ec2b71c17a5fe2fd3d6d6025608b551a0bd1bae52c289a87797be8ebfcb" },
                { "en-US", "6e92c76ab88f2b15e2b02fc563dd500bff781ad5faf28a3c979037efe2d225cccb14e9fbc482aa66b8f30e147ee7f5df54c63f8aeaa3ad9d37506f0b94c49d00" },
                { "es-AR", "5f3b1f83c38bd3e8f6680142d67a5469b5f71a1dd75a1ea65afcb41b55511e8e628de548581ca92ec2dc7a7e349cbe78eb4210becf096c068ae827fafd0a3841" },
                { "es-ES", "ae8ef9e6f98ce7839194be67b4c861c24f1135bde8fea975fe40d65e904f2c8c1664a3503557d46b6ce842f210061908fd16ff0ad71ec4d367045dbdea6f6441" },
                { "es-MX", "ef981d5bc7bcd81b6974774b1f20f29773d160d240989bed4e207f70b1c2087b0dfdb1604a5e96eab9ab1b6d966bbfd4529762c02be272eb13625e980c8a1ace" },
                { "et", "528af2a00d4fd260a479cc40e3de577c2095dd0346d6edb7496a5124dd86b0c181742d3e0935ed24531c736dd006cb79fb0fbf0277747ff8e47bb83707a2cd8f" },
                { "eu", "2df720488a7fd5987fa82d6d11b3663ddd4b5f8f1cb49c3b48f2347fccb41c000da14f70c94d7cd7528e6c060f1b090aa369356b8a0745eaab524ed88ddce17a" },
                { "fi", "f08e44af979471f50db9aac8f7fa23187a0c99baa048770e291e7f06ef1f43d88c1fd1a5d8ad86d3cfcd5d134c90d51efe019fcba8365986db9935271fad03c4" },
                { "fr", "de021581617ad316d5e99842b7c1e84f856e76a7d5b5b5e43c5d59e4dd7b5a47079990f40c8047cc0c56418ebbdfbeefb0ae60331c355594b6eb5147b3e9ceda" },
                { "fy-NL", "2382a8b3d042018776c1b8b034e4c15b61d1f0dbad80217437ce92677cfdd3a9de5680ac0b160fab57bf21ac71d2e2ae5345f38cfc106c674e196d5f82cd3d3a" },
                { "ga-IE", "df2d5a9f41f3f82d428b2186237e542f81e2395664747eaf378078a94757466c004295094069831394ac4b55b3f6f615a5bd0e5a6ab7d0f4517a31d6184627ce" },
                { "gd", "a3355db6f4c0f0a95dc3463494caebac7d382dd677501f889d9eefd40fa0b229c7ea35c051c35f0fadd6cc7fc798494a06d64e1093abe38aaefa2c2117f4fc68" },
                { "gl", "1e7d98d99aa9b3adfff6cea69e1126225e559203e1eaff41a900dce05c37cb3212b5b729af22197148f68e6665dea0cb23db3177843739df7255d546fbfd106b" },
                { "he", "3fe565d535cb336699d81ebc0209f3461e9dd5ff9be855b61d7c1f6ac0e1d6a6a01ae1856a217791e685c6d26dd58363296d150b869c461c552b431e4208fa10" },
                { "hr", "8e37cb292dd8df7734d7f693ab4e26a46d6698aa58fbae5ddb7c07c1e987c5ad94952eac003119c45720ef63026f798388e20e86619072a7c05d36217873f4d2" },
                { "hsb", "1176551121b69d233e2c84cb098e8e87712dcda390c752aac2c65abb74d22b82c58a757add2b56ac32b25c15b193d7395d5c7aaab8f49cb42aae6050ee961e94" },
                { "hu", "4cb78d3b27ad526e902cbcf9a97d39534447370ec9341a295fd62d75d446dc036b0d47b48c3ca57e4ae550dbf6165ca6485cdb616b8fb50a7eb0044546536a99" },
                { "hy-AM", "d03f9d15cd160826b7275bc7986fb3dcbf0d956555dcb54c7c1ffdd01f7259ea24137b5efeac595e8d16cded54ad6751714b19633c5f9f8d65c8320ce0384c97" },
                { "id", "3d7e54aadb8d6b3064da3b3b6f5638aa6567b42e5a34c9e647a07caf5a6f76b8e4abea28836b2ebb6e7a6b237c88cec90f1eb273f1e55be9bb58c71b65ea6555" },
                { "is", "b56ddd71035a3c6f9f47f753a1950b4f80150e9619cf24ab786dc53636d2bea5f65a008d5508e2d182bae3b6e098899a17c2491bae5e49507954609f3cf4f9a2" },
                { "it", "18bdb90acbab13287e7552bdfa68e95e10bb66cddcc3cfd44e724571254d0fb015aa8e3c4b4abdd836b8a5ed8b27bac1f1eba8ddb361b8bf4122fc4fb57f976c" },
                { "ja", "2a99d40f3ee32e0b459aa17d3d1ce1b3f2eb6b3133bc93068f27945dd860b8d1f4e2bcf3f0db2fe93dfcca7addc904cc04dee3ece50964d641e3f90fb474f8b7" },
                { "ka", "b05b4153a7c288808d302ce25beb24310197b3418d5eec64e4d31ddfea9fc63abf8b45e98effea52b5afb88f44a721d54ed2bc1c1f4bc7d85704afb958db6660" },
                { "kab", "a6a4f5fa61d1f99b1885d4ee139f000115971700e2ea1aee93947778c5da6cc43f0094989191c2fc0c42e0854484aff86a9386f2f7f38ae4733e1ef0041351cd" },
                { "kk", "e8f77b40e2ffa68334e1b88bda3295b0077e23881621184231523ade636603b5b57694a8113ad5e01b89b248cbeb611f26a4056f0af510de310c6b545bc4bfef" },
                { "ko", "fcf52ad08f89bf8ae13d44c9ed009180d5ac5f9715db6473cf73c21b6f857f30aa0e34ac07adb041ff96a57810837aeb565773cd8c7270bc1197e53ffb8e6d83" },
                { "lt", "649a55707c0927878a2a0c0c2c3346d9d0ffc4d5321e1ffb72efac8eb2af783aec75ca88b28fe22bc16db032a09ad7e7fc6e1e24c2c6f608a4f75d76d8fe7c9c" },
                { "lv", "bbcc2abf7b5f51f358588bdbc03ba7cb97165e58aee05d1cb57f3beab430ffb6f9ef43f5784a2c2a317ad0f95156d853767f518ff872c219054e454f69ba4f06" },
                { "ms", "d4f85a0fb969559dc4f027d5d0ffb2364c9b89e882b389ee7dcd9ccf8c66490c1e24092ae8144b2795ea17aa2c6220530ec6fa4c33090ea8ba4913b3db99dfd1" },
                { "nb-NO", "32105656c058ece92f30e2f12f21dcf0eda3b7db6baf7579f5af09f638bdb93b3eb9c71f4bcac2b00cda202f33748e8af6184d31f908b43fbcd67103101a8e74" },
                { "nl", "240e8f3f1852ae1394b5e9e57d7f1d5583a4527547928cab57e2d52e29199ae9f403421b6350e04af3a2431607ed5cfde1bb2868b4b9ae69b809a9e26d9fe61d" },
                { "nn-NO", "0753489e3b36ba11c272e51887cb90bd099c86e9e7a1ba397ebb6dfd6b4edf5d5c1a0bd24a9d0b122a5badb9c0c9a0b32c6be137bf30660a8391092922611891" },
                { "pa-IN", "dab150137fc0ca3664b350e35a7054436ef0f2a0140714246ccbe027fe0778614bbff6bf379eb48f07e6b0bc8b110d839387b239ba3df661883a4bd28d32e430" },
                { "pl", "4ef13cfa2a5b49d9a96787169039742ecc1728516346e9793dce8c28cf03193faa0d192cd0751822450533fc5448e0f5fba12f27db72aaf94007c41ccf59d59e" },
                { "pt-BR", "daf3545b36cc7644e9d39ea16257e48356f25121dfad205886ca6da50a4360c770d4827888ec23edbba59c5625278199011c667b35a5429ec41ccac45f739438" },
                { "pt-PT", "03bfb5bc0838f548c5101094df2c675f004c167b5e5016c6fc22796bcbec7c155f92b393192f014fe5ea37a12f56cb96c41961e7527a632a5f62f789df69269a" },
                { "rm", "ff006b64ea4f05280598816dda347d7a64a22a822f554064e7a423a2d15fce0330e02099aa3b0be5ca01337703262886eae2429c078fcace4cc1f4f3ecaee3be" },
                { "ro", "1f866dcf3bc60a613f7be7b683415eae5f9b5c79bf1c44762f5d5c6fae0eaef8b0000f2965cef3c382978d9f943a1fb3a591241bdc07b4846974b42a67f5bbd3" },
                { "ru", "c01665c769d0fce54d351da2ce81d42a84005e4ccf2a173e71ffa2cb602479bb074ec3e091e69fad2e0dbb0ef802f02c675f9ba3ffd75d0cf4cc325344e7cc38" },
                { "sk", "f1a381bf44a9c3706e19e8442b1f7020cfe76ac8b8a7481da3ee242f463fa2e52b78cfe97dbdad03a793dce33b5d3a7c0420d159cfa1104028845e5724795d98" },
                { "sl", "3bb420c8db86ff0e73df1a492ebb68fa05d385079642b3f63c9062f02f3df756b0cec665abef8fd87d9e7df1c51babc3878cda434d6e316fe6d4a4a666e1d7e4" },
                { "sq", "612b8174a37e7017321f976ae036a8cda092c67ac1eb53a5884cb8f031f478f9d8985986e05b655524add2c8cc80197c077af315989afcaa866226ca0ad444a1" },
                { "sr", "ef1fa601bf525491baa2b5669148e1c8705c2b1028d7d419a94823eb8b20c71f6d55470721857490b6b7dc1b12aac066a57044359beffff4c77dcfe5961e2f7e" },
                { "sv-SE", "9ab2374646df1ecb71c205df81316a9f87ef97f89fb8541228df9ee221d3c32f6731f5b0b987cfebc97db6fa80da7ab99c0809e7becab79e1ad0e4c12ed5ddc2" },
                { "th", "9d24b64d43a1e77b622d5412514e1d82bb3b7d0462a1093fce55fbeaadd7a87264d83c49f77a25633f45a22febeec7fa3373a26b21b9cc08abbe98ad876f43ea" },
                { "tr", "76c6b1c94b6eb981b9c93dc9176fda83cbbd1afdff5106fc6629f3fb22b30dd9ee53df7d6bacdde98589456727efdff4720575ead200e035e42849ceb19df786" },
                { "uk", "2dd1b68f1cebff343d49226e3b2cd693435e11e2270aeebee3ee0f66985649e3b9a3427beffd7439b8e249a039c6833c5d20e0f48e2006ae2082fdec8ae0b34e" },
                { "uz", "8b33c3f773b26181b2114b7f21c5fe17d1a8f710f9374f62d1f39e70ea22f829eb73d439e9840ca3d27e84bae52a0f851eee0c32110d3fa13032835153abdaec" },
                { "vi", "0ba2239b04b97c67698aeebf21ad777c406580920bcdaa8e2be4f5a978ebda0d032920c9578d347c8377044ff225fa0af2699a415ef1aac131077b5fa6ce5b43" },
                { "zh-CN", "81aa1e38095a6152cc26050aebdc262206a6a5c1b9df484c5485a8c4a083862b12e520c6d2d750021347f1e1aa10346cec9fefeccfeda530773fbbf910c3093c" },
                { "zh-TW", "8c0622919d1ef85f8676d28125b846aabdcc7e48ab2746bee33fa2c4f46e58bd5cad854362b23b20708c3c5790e2b714c5491d99dc7594172791c62d9bdd534b" }
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
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                knownVersion,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win32/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win64/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
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
            return ["thunderbird-" + languageCode.ToLower(), "thunderbird"];
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-esr-latest&os=win&lang=" + languageCode;
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                Triple current = new(currentVersion);
                Triple known = new(knownVersion);
                if (known > current)
                {
                    return knownVersion;
                }

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
             * https://ftp.mozilla.org/pub/thunderbird/releases/128.1.0esr/SHA512SUMS
             * Common lines look like
             * "3881bf28...e2ab  win32/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 32-bit installer, and like
             * "20fd118b...f4a2  win64/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 64-bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "esr/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return [
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            ];
        }


        /// <summary>
        /// Indicates whether the method searchForNewer() is implemented.
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
            return ["thunderbird"];
        }


        /// <summary>
        /// Determines whether a separate process must be run before the update.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
