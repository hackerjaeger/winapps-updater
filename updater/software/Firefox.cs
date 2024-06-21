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
            // https://ftp.mozilla.org/pub/firefox/releases/127.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "bac2857f74e91dec16b2b82f0d07e4a61b9e5dab67e95788d88f57c6b86dab6e84c0334f8d12663abc795c3472ea3d1ec16249a3925d8d03556fbd37e94863f6" },
                { "af", "ae17a131d0e95feb06939915c00c0011862269b2495d88e5d7f7b23574abc615fb4be32feb56d8441e1c7e4b72335df94400b612d125c12a58602bcb23741005" },
                { "an", "f5f42fd5b3ce4b46212d3e6909363ac020599e5dcebac941b332d496a6b222911438d69a547f4e16802136983db1278cc58a926fd665aee9c7640f4ae6d2288d" },
                { "ar", "35a09cc3f265badd54e55b182c89ccd4c485e98a12b2eee8cc48849426a50248d5b69b29535ee5a31601252b454c00e973241445b94d048ded833bd2b71683da" },
                { "ast", "753e07bf380ebd1ff146924767fda0564d9b6af76352b4c1201404d3df818a39312c0fb743f7b9b6ef62764d4c72395cf94c7b93023493817729d56e07ae9405" },
                { "az", "ff640b5480a81002bdad9e014d89846f2d14ec77b16de3b9830151b388772453ec130cdd90b5785b376cba92543db5ced111a6198281ef9cca15a49f6425eced" },
                { "be", "5dd6204515813b4f2c3075785142407bfc18c86aa82bf2c01b7b73d58c4e38fefec2fe3891ccfa66d24cde4a185d456d8dade335a4c3407c2387181facb96453" },
                { "bg", "643682b286aeb95f56f27c26402992e22332a4ff52c4b8a07a43eee912c6e8cf9e8e93cc13b79caa8c090409081bd94f21fd3caa05825c2026f3f9c16a48d57e" },
                { "bn", "c34779c7f7b9adad7a47c974f96fb6e603174afe50a5f23e603ddd061cbc33dc7db6b4e94ea2508fc7b55dd897ea9563d4d645c05500080c61d9ee38890f5f09" },
                { "br", "530bbe45344bd12e037e399942d59874508ed35f62d37ffdea3a580549d24ff33bdc9244fa45b5f8ffa1e7187763588766e74204570f06abaf77dcae5bc4bd87" },
                { "bs", "d38f4525030502ff4f08dcfb23a828fabdfb1f904ad756d2b07dbe9db9b8f18f393660a930cf71ebbebf43f3a6131a5c5bf26338c86f54701f60a9754be0a059" },
                { "ca", "f9fe7c3336e9bd3e14b359102eb3ab61680af3971fbec109adb87ab2f4c2e317e95a70b4447b59759cc7e8fb31f8e6a4fae5507e81acb4bc97115affc3323541" },
                { "cak", "8a13e64daf018fe29a21bba7a05b017dae6804c278c73c928721050ba70ece6e76971a8aba2911aad56787d54374310b954c674bd4976c02b5d9c1434e05298c" },
                { "cs", "a2032e1abbafee59bd36d0244afcbb05e1e15955dcfe27fc2378e2c4cd201528cc43707ba61503d2dd3538cdf79aae73b7a48f10aaf020a0f2deccc0a39308ff" },
                { "cy", "2d774285f051f17d6e7f3aea38b5f546ba7a54ef4a6334f7b6d613ca2cefc4de98663a5411073cadbabb6fd02d008fd7b34dcf216fc0e5f88cb1a7b1827d4c83" },
                { "da", "9bec4378cd651b3852969dd00e9b8688c693fe418cde96c1095d10338f2fc4deb3f1182a6c368bd4514079cdd16318d04b1e4c6c62e310b4beedaa47d3046c6e" },
                { "de", "7a52e2b28d7d1d9279106cc68280f658520979b4dab98669382e3c17676ae51703e5f5578baf7075ecdc1522d289214ff59697a0bf8379efca15fefc106a7ec7" },
                { "dsb", "4389aa14f4f5b148eb6a1ccac1551a0c5258bfe58ae85b119f27317fd7855dac435fe1cd55edb952b1152638ee420a366decc09a28925fab19d2eb4c03375cbf" },
                { "el", "aef78670707a59ad445497c2b4fa2e90748f5e026976956343a003dc70a9315f32d06d4a4e0828d1829841380f7157885976e0bbba244abc6a733e867fe51437" },
                { "en-CA", "8402a5e7dffa9b88e58a75762a9055318229c48315122a80a631c15482cc5dbe4fab8d8fbf5c6bfffc0f5932ede4d70cd496a2fbee0a8ff8c8929c8dbeb5936a" },
                { "en-GB", "99a3f5451edf66bfb60e3657495dc5d873a6c617fa4fd3ee6ecfe0cc8e769c6d2b868fefa92b603ab185309c0564c75e224842533b20d4e7c77b642277b811b0" },
                { "en-US", "61a4228533d6a27c37039f21ad632ea14ea9881e3cf7437fd76259c78c49239a4afb5f2a9fe38b5373c9d3388902548abc8e57ccca11d9925da36c4c8d30f0f4" },
                { "eo", "475e7b414719c9d3abb0e4b8f28800bb0de03a7f38f597ccb4a4b0aed0d43a9e8f0e6e0ec493f703248cb4a5b85c602e7d12f0780cfecca8f1c6a1d73f784214" },
                { "es-AR", "3a181f1518d061291e95c98ce1674735db30f856599bf94839286e949c84a5aa9a8713b19d417e263c6152967e6129fa3b983d5bb5d51fb0e648b6a282d11f7a" },
                { "es-CL", "7e9c7acf3858adfc42a168851d50e2e3e2da8e7c887fa97f5ba7d7d6eb46e74ed125f21beed1b6d77e57b421940644e212e1183ce9c6f5c8d04afe3df695e8ce" },
                { "es-ES", "4d5eb920b2707dbb6f45a256c29c88235ddbb3da8b72c434162ade1aaf23e8c3aea23dd61b07003fd7821a200c0a7b161b2a5f11000d4c30702dd57116775596" },
                { "es-MX", "79fbfa3589fe5a02ecd812dd2e808ef132f0b29ee6d63da94a54bcf70fb233c341738dd3e9032148e37dabf00ed74c2f505b568e2534de7e063135db03bf9aa2" },
                { "et", "b43f2137b466a2aeed8e80cc5238badcdc9e305609f5c552e5b505826a7638d175d8c5a8fcb6ebf0593ccee948dd9914ba5abccd9373c5233c738b1b05bebfb2" },
                { "eu", "a7fd0fc50ae15c2f44bb5c1d6e1fe5eb8dd010f26e156d604f3d79ccad6486789a2664fd90cd24bc80ad929c2448c37db670948e7dae58d7898d4c06ba77be4e" },
                { "fa", "3bf97dc92a3523158e4f47a02824f39da1d9cafd4e0f10cae864a048c6716bd9c93cee2058159c3f2b5173e6f82ab71f00948a981dfd45b8d5072226146ac9cf" },
                { "ff", "00aefcf22479e2e93618342bc9411b7e7b29969d0aa44833859d5766bfb811ba7af58517ac39f702129f4fcc1a3a63b45a5d4911172839571f73ea88799a4942" },
                { "fi", "9e5e48d9153ecb8620b227652a0bb8e331255300be849cecd669ae06b26a6cf189fd7033c61ff1ac31aa156f5644e25f931fe17ab456e4fd1a89a399e4bc88b1" },
                { "fr", "4b2f232b28b330eb12dae81946e443e14f02b9f35d6c791c690a1c6e322a5b363b8f2f7c15eda718db0183ba2beeee89c2ff26e08e2c09cc88b44f4d05f3f112" },
                { "fur", "aa46e21c486e45beeb5b6e35502a2e8f75beea8c0c3d542e2d1bf80d8e40f308e95caec7833c3b693f0f1b1e84c0d6fba44ce2fd441b5d9d876cdc9586dd69cd" },
                { "fy-NL", "1edae725356af9e3215e1c198831b0fd0d5bec4a9bdccb32f3ca3d8282be2a9fc5eb73aac7740932634e18e1027e75771f9d9f0171f027b99a0c661a5d9b1640" },
                { "ga-IE", "5a741ba0512fb3a78ce681395316aebc7e1057b2a25a469c91668038abceb5ecf7d7c9a5628ce28be6a2a456fa65c1b05d1c71cfcf797d9c98f22fd160a430b3" },
                { "gd", "20ab7a5f037a6266cba5b5a4ebe19250b30af6d435716b6146c8c1805c1ae8251de3b5196e46a2f9dfb40cf587dac2aa4153955de6a475f1dccd106edcd518d7" },
                { "gl", "d2a59cb1126d0bfde3c814440b2a62a7311ce7ae3ffc92c29657858e3b962db9a4ae2f5927827681b8548c3e5079fcc7733714af37f68f222c5aa6e8ae501d3e" },
                { "gn", "912a71b79b805bf052964b071aee59b9e61347b12129d9270202275e241b38526bb0fecdd1d9e2f59574e0603859cc00d4b2086c315c9923e8c0074e2df9d640" },
                { "gu-IN", "89c592835224bc275c1fba953e2bb8cdc0cf3507843f6289e2282a0d6d337143af807fb20b033776eb0fed4b80a142dd666ac20575d755d51dad04aa8bce61fa" },
                { "he", "20396dfc0139437f8bc74f21444108d8214cbf60a8b561869e95a9be45777fbbb8124d05a670d3e8cc80334e14ed2ee035deb44dcbc09149663eae0a62854e5a" },
                { "hi-IN", "c01f78be57af1d6141f472bedb652fcc3bb29c737bc58e4a7236ba3bf4692d8b0e19818afd206dfb6932c14313840e70e5df0ca6732c0d9260241b14fb018c20" },
                { "hr", "8bb4851b01d2e37d1e8380d535a8344f17b16124912e7f9c06f4cf32cad3b2d6489815f0a220431bc4eeec402219db1880e9fbc0b5d2817309fa0917e1db46bb" },
                { "hsb", "03dd210dbe37fcc6040b50a99bd2ea3cc89dbe397f5e90076e7bade91900e9e9df1b8502509961e20a3fcecc640b5ed1e9dbe32acaf572a7fa85fad49d0a6366" },
                { "hu", "93a719a9dce247c98cda4f6e105b6eadf6788efc85eb80e14216b20b280a4d667d6afa11defcab8585198422fba99c0c9dee4634a7d18f7bf6df76cb6b4400f5" },
                { "hy-AM", "507e15ef58b2f282539558e12d6b68f065250c55e2b236c47e65f2299fb6fd7162324292719e2b087d9eca43619ac8afb041df36ef05566bf267eb8c3f685b06" },
                { "ia", "e87cfa3b89a49856fdbd39967c4d81e5829fabdc7f7f6f942f8382021f28682988be146b29ff154a393d07b20e9a4f6e78fb97b3dffedaef0f03ddedef72e96f" },
                { "id", "2fa93badb8d4c7f1025b270b644c8ecb3a5b86689db60d25f9e0127cd5dddbdc3f5734463a9d23b5f22d7ff9dbf1c357e841c2292066d9291b77d4aee44ed83e" },
                { "is", "c5c0ea00729aff9521f4cc545436d6f12fba86eb2c802b154125bfacc4f44aaa585e064464c0e690b0f092a789c7d03354e3c8aabcf4d61f2910be861573b81a" },
                { "it", "e00d525353f7114090d7c6a9113743f852611e67b63a22cf647baba2381fdf1b101270ffd46c60aa32133ac942e10166a36ea1a43291e38897a28d8f5b7af31e" },
                { "ja", "e51f63dfbf308c468e600d3125e79beb0d22394e10f3eca5d9b1371b167a46b4971047e7b26e9c45fd946ef10092791a452e181abbbc83ea576aa560eec0c350" },
                { "ka", "48b94132a8687549514da5255154b7115c1d676e6e5a5719034a1211d26614ecf41d622537b40cdd5db5ff8672f6d07d783dfc9631985ab62b149ae8d8628f43" },
                { "kab", "f2edea06a6509c8a09cba08e8b958350baf2be54ee65e13b0ca33cf62bc585f4436888f170a27d46eb5fd32a804b10a7e54d0ccb432a05b39d06f20db13dbe99" },
                { "kk", "979ec06edc95c3332acbb1d6ee4625fc6bd252faf99882c407c392ce019e91679c03cb413291381abca5de5bcc6f16a8913a4e2eb0fd9b6578547693733f273e" },
                { "km", "749794595a71e78dbe294a5b5ca33a837924da0a797e40c5b19caf97f9afe42d40e0cf942fa03e06c66878dd4a71cc2988f91dbb46412005ba44a55d000aeaa4" },
                { "kn", "25bd70329663596aa517cc80fde777648d4b6c9f4dd6837dc0ce971d1d86d19995654550f65e6b37bcc52b5760c84346e5dcf51bd4f393fce537988ce238edda" },
                { "ko", "ce759186a6a330efd137255dc764454fb760cc4aaa28a7adfcaec4b6146493729849c8837775ece43090d9bf40059de90a2f7c35e473bf7e47923617a431185c" },
                { "lij", "9a9f1fecdec85a61ddac015a7a7d5e285cd6a6566cd3cf6dee09650c4289cf249c3711228191df4e47a245297b3c98514b190e9b0e69ba1d121e55301f7d8ce5" },
                { "lt", "54c11844b69ff0fe03ce380c9a9b83cffa7015009cbd5a935c07dc802e556dedfa628abfd6915f6fa1d2d074271015c637b6b677ad0fb28a76860a704c8d2b3d" },
                { "lv", "0e1d6c5cc95460be6eddffbb03d2de3b88fc02d2bea804be2f0b5b2c60d75661541be61d1c5967ada3b5a7c7621725739d4bd84beb2f39460502c7f380bae8e1" },
                { "mk", "79152994af3d5a67fbb39c893fbb62a15a469bf2883cf35a65e67099611b89069ee15951e4fcde84d7fa9660f64d739d463987db33fd4b94c70161801f26a992" },
                { "mr", "c42054a2b7e1b3131f9eeea31cd0694f41eff8cbbb3e0c2b653dc23ae8b7f1b5465182d3aa19447b470f0fce69c462fdbddd36d5dee0ff9f9db74e395b21bf3e" },
                { "ms", "47507f1ed09b5eda5240326e65f4332116520ad48bee33051a1a938833e9caa5765f06eef9285e69715df9409a283feff43db1c22e61dd746124e3e7260651fe" },
                { "my", "8a456c596b0291524fcd2e4c6f72e93ae35a7beb932c56370d627fafca0866b66b15e1a09e70711cc7b194393ec8bf5aab95eefbaa3e636d2954c3c538e7b9cc" },
                { "nb-NO", "67300dbd1ff66ea38762353163ec565e5ae7decfd398d3e4649f4570e0b67317422a5c1cd121d9e59ae7af2f265cee091d825c0ca3684c4f77b19a8ad4e6bb64" },
                { "ne-NP", "91c2f3ca06d628e75041c7529481574439abdd5353614a8e13c800247275b4d06126fb2857b18bc4591e316f115b3c6d7fce3b8b455f23161102a39113f68599" },
                { "nl", "615c875232d2589361b9f6d78ada38aae221d20317cf4096c2b507006fcc2e5e4b2a315df1e888aa7dbb68ac15fb911fd94b8cbaa3f653930b249f2ab977af54" },
                { "nn-NO", "e0b30a9716c109f1a52e91a985262c3bb07c5e592b4a64f5f2cccb4445bd5d07d02c5b91927ce0d87a5a81ebbb28bf7bb209e6e61b658ead1fbe918fc515c0e0" },
                { "oc", "72f9e2bd9bd086811dfc8cb3cb707b130175d2decff0952a490238985095379b168ed408913c6761cb9728d8d61ee07e349888640b6a9deaeef6d5c46356a7e4" },
                { "pa-IN", "ac2aef111ce8834d4fad02fcc43dc496a8daa6c6ff5d7424492fee700b83344244886660ea8ba4248eb8870bc26e57f5db8519605b34836eb41097be0985dbe4" },
                { "pl", "6697eb5fa21167d44f1d71677253d49677abda58a07ca453d9cba7188eab6e52a14d4ed147a22901ffa4545c48c283947dff0a546fb36b820b0555523b435842" },
                { "pt-BR", "b4daa6e11e813c2190147133e84cb569e6541277dda233b04e2ca9438e4ecef96cb3418b4ab2b46593bfed20a0b47e266146e62015c2fd128ca748f1e60914c2" },
                { "pt-PT", "e1cc31cd0559008f2d272f97a8bae47852bd9ab8ab511c1a1c19b6cb7a5726b28f547eeb8f2d494896d0211d83ea77153081c6f37a303d8964c9fa0acdae87b0" },
                { "rm", "1e1685ffc1d76e78100809818781275fb22b5cff8edbce01e9572c002b1bb8329f4ed6b04d33f4f5805f338f964e91b4900ab3d980b19f2134f461d153a4f12e" },
                { "ro", "42c258ed0f91c6120a516f701fd3076be3423183a4ffc501552bafb8ec93809bb58e8f6cd4972a5b0aacc9c0c8cb77af795d3e96f41464459028058d8ee36c8e" },
                { "ru", "ee0337bc06d42a6c73ba01f97059546c58ad07c6fe67b6003d2ad352f644ae7dc1869bb074c0406fd09da78c588c68c94970440a24e46f489ccdca3a89c1691b" },
                { "sat", "57e4f9a014938de75e9a616557f5495821b60019488eda8abe88e558200ab735de94ef794f18d44a57e09b9c07afafa9cd04f00542272fb61684a9c97d9b4b3b" },
                { "sc", "dafd2b5b257cd95c279211f987a6f1d438906a1ee5688faac9d0f41150b4caa4d3dac98f1f3308a38a8ecb2afca728abcbfa57862cf3d3644fa708641a1580dc" },
                { "sco", "e0c19eea16e4f6eadb53e74913de05ffebe2b94d320c341ddfe251a9bb000d04deb8ddb7901ea62d05eb138fa592382044c72d3b544a2e42c1b5f1e893ee4403" },
                { "si", "9ae6bde30d2482f7466d52e0c4d1334a196d27457a61e42fc2286c0a0d7d1d27b8a2d40a27a04c971adc96a5136a7b2282ac2bfb52173c482a191224fd3b78d9" },
                { "sk", "3b69fba2819d1e0917aab5d30cf1a4e35f8e69dea1bca3d7bf928bc618c58600b40b366ac1a38480de776fcece7b50d2c6208f66146f77fec7454e78380ce9d5" },
                { "sl", "607250ee56bba152883353555e1a540fe55e7ede7cc744c9a6f5593d01aae6f54533abfac6f40a13202f63e75435451c80c3d94b03126c9e76a3b987d30261fd" },
                { "son", "34783eb014e4236ae077119629f44a824cec3192011e5b8f2590a706236623af8de8a31b71c8e80d59e0034aa3bad3e2856688afb9f96828c521c409d3f2b23e" },
                { "sq", "8358c25fea9a7f9fc1dc8c9b97aaabc35c3762cffa60b47b46e138f9b941c8c73b79a246f2f8a9669c3ffb038a66f1939377315bb874ff9734311b05e1725264" },
                { "sr", "2660210059e5d2080cc54becaf7ff06fda9cc6b0f5279054def5a252fb0e75420f1646af278f9a4194ab36cc551f6d0b49e0a4b4c3ca61aaec3408c344cfa1af" },
                { "sv-SE", "060bcc8478757414270548c3897770c2ab0e0386928e615282567fc4eae41bd8c1c5ba34daf7aa779c6d45e7c97a87c0bd73108a9bf5224ca3bcf7e422e132e8" },
                { "szl", "a757af3e2c99e212ef6c75c1b1de7e62247f7e92cee058ae97246f4cff9f65b316ab5a7d00f55d70b38e32cc7e756c907c9e36cec730adeb84e08ad804eef086" },
                { "ta", "45f8588157dc28e7ecf06d09a60c8ebf10490fa5eb65c6fed24c836bd45eba1b2ec6aefaf4255d2082f96e846a570dff7ac13c66ed315a72a5a0bfb0f950ccd9" },
                { "te", "a38e882445a1fdc3bc67b8a894bfc323b7b0241c736ae11329f2904f7f3b572a9206c9c3379a91a10a728caf2cdc72848ec38d5b880d384c6212d05c94a4d6a6" },
                { "tg", "ff0740574dcbc933be000c8d1ece917a2e6c0e4ae6025331c69e7a2057b991aa4eaa8da671edf02ef02e224def9700708ecd9c0721f2e3784fc4f9f6b8718756" },
                { "th", "2316fc6362008326c6d9be19f5463d55e8be2c0859ad04e532651a5badf0b630f42c326bc4e287408e132dadcedc38bc1e46f3d861938e79e67e3f93a79c015b" },
                { "tl", "87e444ee477a5a02b3f4546f19dc5b39bb286d48b374051e8c0a7134ffa9c515e9974ac29ea92e2badef02109f713dc5aa9e38eb2b5e4f0bbae2a2c721a3253f" },
                { "tr", "ba9d719723263564ffe8a502f6abc00d9577d73753f7d6b764d51c852a82728fc0b6f8638e0dd9169dcfe8ed7ba48b07ad4b43f19e3b40b62314e9693943135c" },
                { "trs", "7d8066adec18f5e9e871cefbcc14b4d3a38bbd468ffd75862cec3d440036d43c41c2f9ee76f390df066bce662d3918c7f439917a352b820e77a4301b4de816c2" },
                { "uk", "e692eb9686ef612c0b9ae280791609480fb3e029d84e064a9ecde354f6a0dba93f1f44177d5e3e58cedeaf3d71548b1b5bf6e55f7b034bdea1df7d282298b116" },
                { "ur", "954a155c9d8e72e0dc979be35d1bff1f2716a0d19c7a293b6d61bbcb0b2da271575ef10ebceeb424e59fc1b453bc2bdbaef63e33a99747d9f084391764d56330" },
                { "uz", "cbc102cda7589cfff9426df776405f775f9a88d989e36f3b91eb53473cf276e4a53992740f574b270ecf802c415e157a65663e62634ea5711b7c4b84809a3e98" },
                { "vi", "08a15cf7fe39aeb5dfd24227dbae257ce38fae3d48a87a2fa8e2d6ef399406cd19a91c4b49560c64b19e5a00f1cca10a8d4f4c61b59b4e2ca42f75531c7c7107" },
                { "xh", "79dda3b8d60bc28deaacc46c1215ba67127b88a4e18e808dcfaf0e2b96c9e673042355d75cac0005a8389e9f65961603ffeea5d02680a7554f432b0b60bb3fae" },
                { "zh-CN", "3b5428e343521e65baf2518bce3aca1c57dbddd229712741815d492b8f18c83a0fbf066f11e5745974af59c01f28ca942e91b9fdd7dbfe92d1a31481b861462c" },
                { "zh-TW", "c5180976427552a1fc5bb9eba6773d5fc4150940615273552ee1282c62bf940c61020c9f94c6b60edd76cd398195bd63756031f7dcae2a04fb7ec4775e7b759d" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/127.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "1176302f5862d675bd4380dc608b0aaaa6c814a54f9eb47cb99640cd0ae1594ab9b9f90f910441599a1ac48b3e7a9a4cfbcc95977d3f5de0c10a8533955014d9" },
                { "af", "90908a028d400ad47c97442fe9d8888c0427c3ac8361175b87696fc1616e0335f8614ecae6a80a8ff09bf205e8931959f177e1cb9545e18a4410b3bc3723edf7" },
                { "an", "f3300aec4eea6408bc3ac3368277e086ddbe62db03fc210cc2cf8a0a418aebeae7600c46444330f2bce445e95af914ec20111f352a9482b91f7ae4254a8f0f07" },
                { "ar", "db748ac93f10f0702a0be996a3f1088d6839b059d3ab4a41887dc89f2d50bec18c544b770793bdea662c33a16e90d88544373b59dd909e50f2cc0c5e597161be" },
                { "ast", "dd3a7d49bcbfec418e5acc11759d6b02a723e4a53b4010fabe94fa9aab48b7604d4c519205bbe68f1cdaa5999e2613385044e677688bd2ecf66f7f8e87423eee" },
                { "az", "dccbe1c48f519c67fe71bc9506c9d42d2c134f8f6c68771123948bacc328ee6b91926130fa942957bb094dc006ba18c3d00bb4b61ab0ff136ec91da5f19bf347" },
                { "be", "728ca716209edaa3c66cb1b3a8e260cfbed3780e6078de223eea90585a4090044d4ef3e9dfd53aa355040b6a6c16d78f2e88260b1dc2d7db1ac02f11f21053e8" },
                { "bg", "4ad8f0afbca698c145f0b94dca40c1bc6243fd9677c2ab5e11c577a9b4efab7291132345660b3253bbc5462b2eb1fd882f4c06a0b7b69eb20176830c8965061c" },
                { "bn", "656be434f75cdf193c51cec1f2130ac0a5add66ea8e9a18484efe425f5e6648db850484f141f71d869bffd94e7cb4b45e93bdea050b1898cd7994713ef236499" },
                { "br", "331255c0537851e84b0cfa39889277af1b424a786f3e2c8013f06eeacceeadb6b6846b7ae3e2081286718aba9641248544d1c6516cfcd7e7d0cf73a160dc6062" },
                { "bs", "68ff4265b239291d0c70fb20809431b224517f97143745818c38efa2a8c06c391557d209767925e67ed9c4a2560c947ddc59b829ae42b3d00815c4ca6e59223b" },
                { "ca", "74cb4bdccedee786feda779b7b992017c803ee96871c3e22c521c7fbe133dc16289c158b5ceb6d5ca753f7e5937599944861a131d7822486ecb62addbb9505e7" },
                { "cak", "762244318462a4edb7dbdfd4873be75af32633d2708cb490787053296a7b925406dbbadf1bcb8d2eb7ca40f14481b756c591ba1564ee8aea2affae43a98064c6" },
                { "cs", "8b55953d77e27f8ffb6ada77fa0caf20235a3cdf31c0e27bc671fc031e8251d32cf56fa4f47aa4282532ea44806ee7c5e5d16e6048a0e512d30c4ca21a1610f3" },
                { "cy", "f266f8dd2f93346f3977dbb3bda935cc8393dba5050df8028ba2d749b486ab6c8b1e3e0a64847ddd330fc14e2601204591282c154336b693d0eea907c5c7ae11" },
                { "da", "e02fe6e3bb3cbd106bc32eff591e8e7a46b8b717ae8c355db22e5faede2b840efeecaff2b6a1876500df7b2c10f71cb1ef3cfbac59b6c997e4897e70db54beca" },
                { "de", "694790333c218eb3f3a7a1ef5b1bc3ff6ab04c9c818f3993d13cefd72ea618a20402d30800da6235ba986d24308a7c0612c7cc5bed16c17f6cb02d38b57d0d34" },
                { "dsb", "e33cf53f332deab65c6689790bd101bd6b3635d560d590353bbb2b605cfae21debc01e8c8d9144705dab241616353b43643f0aef11856b5e40e939906134bb6e" },
                { "el", "a240c849547327956e2bd09032bac077d6f7579cb9fe65714023c5b19fa7e0094cba2f7fa567ba8ab5f749b2abf621e6b2ba61fe9871d67479b483d9cda0ca15" },
                { "en-CA", "14607c3d3f1eb7834e082b6951d87abe67b795901464b8b628ebc9c6fb27a59d6b756eee66051a002a9c68bff59f20f3cc2354da9a9aee8418879683cc4f2c8d" },
                { "en-GB", "7aec9e4deac90324431fba1eb94eb4e9ad6be6151d18f9da4d6caf1e33d617f501e57e9d179295cae6682f23d2c71b565dde0347660d004e6d66006a5d228cfa" },
                { "en-US", "676321216b2a4bc09efc57efdfa51798f576e7867ff563f66cde9eef2b64ce0d00ef5ec8a0482a6e554075b97e6fed246d5c61a015309eedc3f90828d3f122c2" },
                { "eo", "29837b00fe33c02c6aec01a8487e8fce59d4649aec5ac618acf625a30325b78f29cb1447f347ca3170cf47a9d71d4c149b373d3b54b2492da263f59841a8203c" },
                { "es-AR", "5f3131c97b834db69eade259b6380f120b1fe6f8eede35dfb893f5d5fb7f4e813d38da65475f6fe1ae61ee78d56e13853c3f19a24ddda37a08889a2ecaaf00b0" },
                { "es-CL", "6f6e86e21b02b25184c8824d9af72afb3bd3adc49f4daec58b4f532b796eedc4dbed86c39b80024cadbdc8714a48104391be16abcfc3eae1dbf9585f6dbbc3ff" },
                { "es-ES", "880d816dcd922b994ca0a756e3719d70e9297d94c05e32c3b217a3fe50664ef1505059ff4270d20e453bcbc4c2c7e0d75ceeaa27afb7bd39983e482d0ba692fc" },
                { "es-MX", "8cf5aaa8dfe4a30cc4fe275d579c5ae7f45d46c746b48f4c5baf16d5e7d55304f17c34bd444089672e5db982b9d7cae068e0ef858ab097bbc2c5dc0ccb57121b" },
                { "et", "cd181a5ea32eb7591d54aa4481b687dcd3738ce780d6311cf318cdfd6371f306ab9e11359f1bbfd9cd037dcd05bad545cf42fce5f6d2aff6a84901874cce3025" },
                { "eu", "8eb714ecb18b4a247446b4c7e982a1557b6d3c35037446ff6580231e87079171c1616033c6445d553c3186afa229c8c59bd0b6850e256ab3d081020a4cc4e3e4" },
                { "fa", "a0ee92ed968a9b5db18cab07914f0ccbe840c38a572849e03a3ced934540a19270245705a2f0f1b3bb9b182b1d13e9e46c69d26aaa6767357f74a4ede29abb79" },
                { "ff", "2570df393fe3f14478a9e328514c24cb27bd269809caef2d15b55086d7a69441854f4dea7b3f1ad67cb59fdc7c109fc365d59b9e6ab732f2d4637a52e520b68e" },
                { "fi", "4e63a89316f4d5dd1ea782a9cbe2a8299840c6ac2344e779330cebb83c8a07457dcf2e34bf2f07b4c6eb1eb8ad6988d69b351068112517862324be6260b5df6b" },
                { "fr", "03c5ff754764f4b682d2c8f11d92806ce94593d10f962411f56a8eb32369bca4c225122623419a878666866ccfb4c7aa8dac18d35de77784469a96d335fec19b" },
                { "fur", "11abed57283f6a445184e1b2d36817930af9bf1a96d47717c369f679bb86bbbdd2bdc83c19062e065baa36bdbfb1c57829fb91b4af9b3f341953db519108893e" },
                { "fy-NL", "b5922b66bb470d903eeac4d7ac2a8e7adb441aab8c11afa9d7cbb6dc6113ba19f0be97fc7fa03c69723cccfa729d5f7fdaf8bc50db4ffaf523bfb2d0cd196b89" },
                { "ga-IE", "deb0dba8905f79ecbef5b50b657d29f637b5b5e7e9631906874044a272541dd2353cef8c3d09df02cc556f5860d236a91c73098132b58ad7408a899f178802f8" },
                { "gd", "0f238bf32b3f6d292c965e3fee7fe6a6b1863f07788dea5857b4ca7d9fa7202e1218005b29a67182f9ba455a6c271a0d858141b8b68ff0873fc75ed12b30d167" },
                { "gl", "62b7e02efb1b3a1f71be277277541f702bb86ce26b67aa632f80bbe95280c9e7b3440d4ec5edf194dc8e567c38da801a0e75f8c487d5347c4092f26d9e3ab2cc" },
                { "gn", "f5942b85d72f75d3f72817cb9f86e792595556fcac6dd27155e41d8d986e68fd2e557339d7a12b30c95c5526dd9af349d6962eb9dddb0e7ea0b5b7db161b34aa" },
                { "gu-IN", "e5b68f0a2968156a0531e6e28cfd73bd55043ffe42f5603b232b31e2adc7ca289b236eb798f0197168a70a25cc646d8acb1e68599d027a941b4292be7cd82394" },
                { "he", "7899868de14b3d9fdb61ad525ec028ae296a0565163666742e56a2f8d7d172d842cb8856a5877158d4dca5a54282382985e361a62bb0608a4f6c10c768a79a96" },
                { "hi-IN", "3277927c554bf899f9c2b28d95376d61c193f09b40e3eab941d51746962dbb93ef434693021c82699f9a2a195fd7b110946af97dbd3ec3b47e4d3affeea54e2e" },
                { "hr", "92c16face256e3cd7e43a413629a06a5e89eef54c4c4cacfe0062bfc8fa22788b3cc18308508ebfad65840ee3b9e137489975c70b478407a94ad3a890eab443a" },
                { "hsb", "929e488e150d56f1c400f4d442f5c034a327dc95d07afe78615bf1d529d29b90b1136d742c39ea3a9afab2df4472a012e9c5753717f6e0fa70f5c97a8b3d0efb" },
                { "hu", "479ec1321cb99bc4a9392251305d0aa4d2af681c49760661e52549836a24a7480d1f7779481cd6453d755133de38a69a718ab793ebab347abd5095085f4e68b6" },
                { "hy-AM", "c09d122761505e28e8430855b726500543c9333cb398777d1216f2033f47ae6a6a8cbbbb7c9745efe54b43e9c2a368515a06e1a9c4e26c701d2ef5277e6379bb" },
                { "ia", "0199216f6c2045698f7d820b2eb6362423954f916a0edd52e5d3b77494e8681eb11fa56df31702601ec8884ad5dedc2824401daa711bb2d2def45b39f3a897a7" },
                { "id", "99ecf3122ac4c64897b84f94f6a26d425ecdc25e9f0e2cd9dd64309fb29f3a188b3aba3156d5723f4a23ddcde243fee04d16a62027d18648b5ebc79dc19dfa0e" },
                { "is", "a889794ab1f6cd7f6463beafc545b5ecaf2d4da4a88775c587da41f50361a86a4e2f1d88cb0d8e2ac106c057c3c0fa16b836838793d542f779ea2b530c2f182f" },
                { "it", "1ee0cfa21afe6de750566cfdf509dcc545e9e1a0e8f1d4b7b9a90448cb08250fdbc7a984e8556b5eb412b916edfd246e9f1e59c5b6366f008c818c79ec136d35" },
                { "ja", "c8b189c6609ab3a382dee63a63dd8988d4d49479a3a5d98bd9ac9b870f6912b32dbe08a12902b17cb98bf3c8bb307303ef05d7e57e0106459e8ddfcc76ba76f5" },
                { "ka", "7be32a2db5d92833c2e4ec8773f3118a2e84616c54aaf76f32ce4f7364e3a1e3c7a95b88fe60d1cb0d88271ba151e232ec27f432e903664159ff37854d690858" },
                { "kab", "9bd0414ed4c91f5cc593181fc374ba6a9cc3463644ee62724bbbc131ee673b0614cd0a55d9fa5cecaf39715f535ea6b529aeddf44d8b11399e8f556a59959911" },
                { "kk", "26f7af674c69aa7308dc46beb26a60ca4c16eaf2218d2af3a167a8e2051c56cd7c4ada138ecee1f04976a5092fd7571a3940a7a9ea59ab1c93c277805ad4cb78" },
                { "km", "777a81b9b590c28cc0ffb065f341c3f2ad9a1dce820267c17af0e675b5bd5ac22c6c5293f2e19ef5dd59680679cb457156aa2872640a0a86a1925876bf04cabc" },
                { "kn", "1a7ba4830c49f3d0a46b52f146a65c3921ce9ef1efa24f80c923a8165b43433ad6fcfb299ac9cef9be20eea27e7e0bda720bfd976bea6510da6baa4d2ccfb08e" },
                { "ko", "b95d4485a355086837a0288b53b39ecd800a6931b969c5f399c2d0512631e001a541346824f2b8caab42d7bd364b25ef4766a3ed194ddf70ff85138ca017f9ec" },
                { "lij", "9654fc0003fe35ad28ccaa27a1571273a1f7e7faa64bc84c8cb0a4b863731598032c191550a6d0536c91647702f030f0c14208fdda39e99cbf7a227046089678" },
                { "lt", "1e58918bef5f4b402ce4779776a7013e221688b8380083713cc90ce38405d05676854c4338bc07ceeaf1a4e3d55ceb50ad793ba8a5b6bad4bb26df50fcb704c8" },
                { "lv", "61b537843cb3160e27d702512f8c2a1d6f67a85ce62749a98561911637a731de8ad4ffe4891f39c65eb006125e6a2a8d2d538f12b214d2df9dfee5aa0a09099b" },
                { "mk", "30c88f4cbe5b456ef2e645ee67743e206c24509529321445e8cce67c65d0a9d051d56cf3bfeea31056c7876b07ba3742b104adb196729eae47120253f18aff35" },
                { "mr", "d97ca2c2e0c128af17cfa5b4f72af3731c028c801a1dd81f73ca4eaef8fd7d5beb39927f400e1f2201ff77d1350c2eb8f3fcd191421b11b37899a2e4fccb99a2" },
                { "ms", "fcad60495d281da4f19147b87328d8fe7bbed55ae64a3844b58e4239d2df46f54509d0e1b56ac09c8c8dd356333b1b648d95e9a91565e9e2d9c8909a9bfc92b8" },
                { "my", "71935325edad69f8867873c937cab8ed57d432b5ea749b44a15d1a4c4e078feb46d2a82ded6c6661450056a259b1bff55f60061637c2fc22d77c5bb3e77c740a" },
                { "nb-NO", "d15f07a06210c1ebfdd942a47604ec25796a209913194324cc60053112d89fd766e49723cd1fa362b0f0dbfab73ee78b6a52949b53474ac7e1b03149f56ff126" },
                { "ne-NP", "1edce27c013951054f558f149e1baf0c9452389f4313dd2adf4e50576af6d1205bd8580645e7437fca5a15e09d310cc944436777a669887e44169f823a661442" },
                { "nl", "05bb4a02e62210eb837bcfbdd7da00fcebbefe4d62cc4daee1aeec51d663305c49b0e7a193efeac800b1dc035bb1df56e3b4c3ca0456b9b269f275d072d30a74" },
                { "nn-NO", "c66c6679dc668dfd99461b4c6792e84f0af1ec46ce8e961128177cfaf9532ce3147b61d60f3b59e60156e5392febb7b38784d7b1b83411917c8d921ebf8880b5" },
                { "oc", "4a1495e0391266de4b1f565a48f70e03667737475cf5c509422a5cb47b931fb209110374032d5129cea4658b5df027a0a9c7b3c6b670d465135e9dfb581009ba" },
                { "pa-IN", "6edba785eda398e5fd30f51da3a038f24b2776dad6739a6f893d901350751628873fa3de66ead78c38bf8a48d04f61742da31c0f06916b0cbb2dd028b75c0144" },
                { "pl", "b3c31d69cf826feaba26f08b5687c202407c1ec36461c057e4bc9ce26c22ac6b727a245e33cf296d2513b0540602b72d066867002888a4e1db298c1a55fe2594" },
                { "pt-BR", "a566de8348bb08b0b272ff433b17df108d17757553abe4eaecb953a1fa0538b49f5cf9d846fca8535947b5849ee6355e63a7ecd0e7e09885318d2ecf0029ce12" },
                { "pt-PT", "bed0f39cc7187d3f47250268890a5fbfbba139734ef60ff82a74f59044b23fd1b5059743e872691260de9b59669d53d5d62af4755e95a5cbe2c8c61197460372" },
                { "rm", "3040f704803eb799f8e8c9ca9b0e6150c8d29652e2a616b39c6e76cd8bc9c24e830ba6abf7576974dc872416a9a8ad418705228c2e62e48616b4c5cae2dc2412" },
                { "ro", "5ca5a44e4762b183b90e470fd0dbd2213563c41e2decf46263c9fdc648cf84e080d560bad01eb6ccb6596bb6e66cc6274a84ec482c1ca02667854ac9a3cc67a2" },
                { "ru", "412dc0f5beb5f38c20950937d087674ed6a312d076713f87d33ae8e4206430e83fa143494e2685b283da9d82d3368b23d89ae14f62e5a7ccfd217c1fd05de695" },
                { "sat", "2a5f4fc5d5d1e9c21153018c8b132c13e1991061a4070bf2cb88520092a362c7b6e5c29b07231fde4c2801852f8d7a2c4276fa4f99a6f1aed97208a292e4ef2b" },
                { "sc", "44609e7fc4d4f14571e382102ddff9509bf90786490ba4ace8f3858a0894da95ea39a849adae19736cb0c7809abbe61262d57648e5995babde6ca31df4199698" },
                { "sco", "ee1992b21bc6bc72c6b9cedb91b62f56692bc8a9eaa7754705f7ec01c68df734165ceaac64e52576b159e3586fbfbec9ae5d27aca056cb3b32ac2f7e69824c33" },
                { "si", "887f05c5f5754e292348cb2af24e5de90b459b7a9448cd6be8f47c71c4f42a9f8e73c13024bb371cd4e242f63a5f21191951423e2f0c1ea299355c583ec4abfe" },
                { "sk", "870dc802fde5eb32f1818b8cf00c510cb2462fc9bf5fbbbf948ff016f40de777a76b351595dfa293350fe554362cc0536f06469b7a8f5126f4e4f4189bcf505c" },
                { "sl", "b0e942c6265f0267452597b13f78ab5d1074fa70171e54e5f1622aabc56906c88330b7dbeb5eafe1a44824a53ff7134b7ada476ad2533d13a5054fde3f76fc88" },
                { "son", "71c8b286f8747a6f0516bfa398f689d52bbeb2b3032841789481094b99494f74df357a6822df73d74d60e9f36d277313f2a94cdee2a61dc86fbc23fb6b9e42f6" },
                { "sq", "c40ebcab5ffcdc317da60ea7e88dbf190e878cb619999f871d5616a0d4cc80898c229f16f139e9bcc7bb615e651cd906dc543e099515f1fd36688144f070b793" },
                { "sr", "05fb6e0ebe55e86a9ff1f87dff33c5d45d8c95440bc0df94695450ecfaf3974bbc615445758438738305382ca68a8509bb4bab278906f010797aab7886aaded6" },
                { "sv-SE", "df42fd9ecd432bd78e82478bd6060b4939e43bfa5c9ae5b4217e5e3b8cd2713e2947e0ba16baef11d27718c85700267b2f6809d709e466ee1de150ad5fe58469" },
                { "szl", "11ec6cab94beb21777f558e2567af7e359530c61e56b209dfeda0dba6f1173eaa7b8af64388c07a823aa9d6c5dcbb0d5033f60c81d828e973047b10fbeea2adc" },
                { "ta", "98b3f64f8ab1ef7f8a4a1a2e073fe499470e51bceeaedfd14c96b88998c95d781ce6cca7390ab6461a958c5d9f094535965480634e7247f3d213bf1317ae495b" },
                { "te", "c6d603fb01b6e48c1051efa9000ffe8d368011ddd3aee793505c21e37649388f64445df8ce8b3e73c50e218e644b5a8c42ccd2a16ed26275c4a3fac29b1d574c" },
                { "tg", "78ea50b159af7e2e53120fe623b158bfae88260b618541f057c4feaa8fbdd8d9e6156fa63e46e3e73bac2f8634e3e8dcaf5de97870147be663420db6f8d5ac20" },
                { "th", "84014950e47ee0ef89b7f4b2edf5e468b050436e0a2853036b7d2476460e0766ff65781694d560a6c093382907c68599e963c69db4a1676f4b1de649cb22339c" },
                { "tl", "7dc69c4ca47d0f40b912bcdabef327ddcb3c969c6cc128a98dab4547228f66fdb2e939a3b230388f1ebd134f3908b1a95c2cb36d0791af5eba380ee1dfd3850e" },
                { "tr", "5cdff45021179fba803f2b37e4ed27b9642ff70df389352d2971b49138e81ef0634d2a67fe78ad1c5f1ceb1dfa44f5110b877f204c14267c9666f3a416f9ef18" },
                { "trs", "3773788d208a1ed6e98c6f55e2381ac37a0af95778c50a06060d34505726b6a76baf184364884363c23e03b1f070a8ef840130c4a764159f4c7dc756ae3a4739" },
                { "uk", "5e5bcba67488cbf2d387254a30f985714cc368edd792c823fb6be34c45a7bfb3cb2a5d45dd45cec53675a6550f6e72841168833b10a806466c139cbb0a5e3556" },
                { "ur", "bf4b00d57a3121feb299f06df0ab8d5c889dd741268cfc6c087b0dc946a8c7637a5997dfe6ee25dd260b3ebcaa6cf372b850621b84505d8080f10b71128a692d" },
                { "uz", "11f582cf713fcc09bb061949613b853fafdc37553b3a959cf368cdd2ee946ae4144db236f25f866cb3b6fc56c23326ae6672f6277242fb123f75016b6d1a2ab1" },
                { "vi", "d113001eb423687fe0f6d9b8adac757e0604144952f97e2933218c9ae0b29d3919adba529a300dee8f4cf274ca8dbaeb80afa69cbec8342d227dc1201c8c06d8" },
                { "xh", "b1dce7e17dfaaabe766286c60c6008bd4d0fdd0626502b46cf472a983083543dd285f7ec9ee4a992611ba7c2bdf1bb3d04b38c2b5c8213c6316a5aa1d1e90a16" },
                { "zh-CN", "c80a2a7a098883725eb703cca5c4a133abb12813a67e977cf2f004f41ca83c4d85fdcb2a5fa3a862540490fd67224e88a22e2e5e2e155867c6e651eaaf636908" },
                { "zh-TW", "8d7c47457da552283e8c5b0415c8cb6b4c6581baa85fef2004ad876b39a25add92a7270936afaf2980425c5847b3bc32f7036285881f02869adf1d2fe3893d8e" }
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
            const string knownVersion = "127.0.1";
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
