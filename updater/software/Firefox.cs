﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/105.0.2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "8e5e273df968bd7a971555713525f99c479d404fe5cf7ee961e3b09c21833aef309bfbae0b87f8a1fab8566707cd5042d7c3ef34148bd886e55408233a00ecd7" },
                { "af", "d44f2ce5adb43936a431399b1bf3226d93b4577ac4e2241ad9f3a7dd40e69dfd55a3463d43df4b862f424680f6223f6036cb6c5642f95fcadbc07b673fdf152a" },
                { "an", "1df57ae1cfabe07e12288a8447320a9ef4eb520e2e01d15aff87f478b1937872a426ea396ea9f9b059262ac063b001deaa900207e5c12848b45b423cd3c0b9b8" },
                { "ar", "fff137a5e787a5d0c5dc6c86d80e3e4d0edfe0b2649ac9fafa64aa9b5b2e7d90833d6f208a3e4b0266f685e7dc09eca4a03544dc0b25976745108f86468641d8" },
                { "ast", "a35787f47a580ee844e2a13813e06489704d41d44d6339bf497e6c5709ff598490e376b4371ac7ae6dcf81cf7d464e7d04d4192bb01553ec6999e5b13d4ec83a" },
                { "az", "440c466b7e3d4cf6f374b2527b9faf2c2dc8146ff68ef9cc90ac5620342b5b0fe35a9c78ca348592687d5e1b9d215b7533d049f2567b19b4614bca3744dcc86a" },
                { "be", "4ee1e382b4b42e847d419807a2ff05a9a254b094bab883ce59a90c276bc88e5bc7fd38dd1ffa9933da73de1d1a92921a3649b48d51c5c41f1d65a49dc76c741c" },
                { "bg", "caef90ca5a09783fe8ee279957f2950f7331e7a38767ce8de875754da252adc45ebbc2639abf3cc0cad594c7c1dac0547760d37d85c6ec7f22cc889b9633af63" },
                { "bn", "4ae8d4a2e63cef924b3f0ea79b12f17584ba2fd3cfc79beb42637b1c049adc3ae6bfe168a2f136f9c282710002cc685fed515addf3703a0e505d86a15b77dc03" },
                { "br", "c2c2ba6eabf0f78fa9b803e06fbdb7f69e3d4606235a5467539bfd016c26ce106dbb46daa2cd55edcff4288ffa98bda859a5e226e317becefa9fe5389563b048" },
                { "bs", "2036d415e7edeb8cb55dc7e62190bfefbd25a2181a276a4c22f8c027d895f12f22eb7d942e5d727ee6bbf9d017776f5e76300ebc3c606231225eb76318d53007" },
                { "ca", "4b6a3b2a49cb3b6c70563f63c40fc93a1636b138573b573c9db7af1f9e6326a58f01b4f0e7c78b4636e3e878825224f4283c10a49792975477c1374d88f52736" },
                { "cak", "d306351f7d90c1851d8a06ce1afdf51ea537abf9109046559ae0c43965a5d0c30f8abdf61ec37edf51ac8471aee772fb8a1484d30fdafe80a8a3faedd166b350" },
                { "cs", "2363decabc44d724b3beb7144e80da6596b7126b11b301c861eaebe6b9889e1e89af1db9d64de850b6c63efb915b8d8d0a67934f80395f8aeef38833df7b4a65" },
                { "cy", "8908cd3209c00475265e662801d28e7e5196f249601d3b1c40934911dbb6bcd2fa222815ff5168d25b379de9223debed548231d50e9c80a9987dc443e58f14f0" },
                { "da", "4ea6406f826ffe493b84f15ef9793c19219dfc4e968fb9e69cc28a576e78f4a9370fdb7b826e3616d118868058736115035790f60811743ea3cea75c50a6b441" },
                { "de", "3d4d3aebdd0f462e1210fe31987ea2fdbc28067eabd7fc7e9c8fea1245b9f80d53b16e33d64ec2fbb63e65553f44fd0be4872a655329bc66999e2e95196d11aa" },
                { "dsb", "0d799d3b1267af4865b951ae5f5f1c228c42885714619232ab494af538e7cb324477840f017e3f357a2045679d429270b5e08eb790228371e0b2ea613d90b554" },
                { "el", "a399098bbf63d8af6e825ad6d1d1004e1e056ee019fd72794d6535c06f8a0a2a7a41469f89fd4a9ec194b835460eefe27079f236060a3d7385024d8bf24f3e3e" },
                { "en-CA", "7a53fd8bf59eb0a964f4773368ab273a2f6a7b4f9f77f839520d022012b71df72f72ccd49d02437ce0deafbc331509249af91b6dc1a726a567ef95f418576fb8" },
                { "en-GB", "01aebf95c40ae94def6a3aa65f6e94702af79fd4931627377697b9f84b07ecfeaef32c74f7fb5c0cd00a72b3768325fea4fdf255dd917d3bc2a063dc398795e9" },
                { "en-US", "be99a6e9c714447e0e8e3e449581dd7b1ddefc48158264137260e0f60bdfbb1d90a7af9b6eaaaa76b8a7a7a5bfddd3d7ff891c9b3120ebf3a286f1aa63dd041a" },
                { "eo", "83a05f7be9b279b0428360ad7d6f2c2df36d968387a5b299e384bdcbf5fbe2a453b6bf36d1cca259fb31541f31a765465b75e95e10d5795b695fa2bdd6f5a468" },
                { "es-AR", "0f06dd4a63927fc2fbb5eef0cb7935ebcc1f6a070c21f24e477009cd1b3187e1f7b0aa4dc62acd5e023cfed0e524201df5264d492aec4a8206954d84afd8a4f4" },
                { "es-CL", "5a1d4ef44a34c9ed3ba08b317079d56784da357c73fb654f2280937a1ded481a504d46ef4c3025a8beace1b8e6356e8d35702d7700d19c099109b72eb264b2bf" },
                { "es-ES", "e46cae4a0e2ba00a0bd2fcc296dea6a37438b098e6e092fd5d081705fb33aaf9a67aa6727aecaf63633b4d2a3b0bf3b13d1c3794f371de84afd657170fe3075c" },
                { "es-MX", "25be74ee85b40b2a4bd692ba3c03e328baa7b91ccbb7a2365d97635b102927ca8de24fbf22c0445c21f34fee7092521be2eb42cc5c8ed47af3e5edfeccdf626a" },
                { "et", "1b7db1d1cb88996984c0dc70871df29a5ac93029a255d1557ef79f7ad3fa735878698cc8f191251705982d8d36a3840f87cc823455b2fb99bfa7442788c63c3c" },
                { "eu", "6d9bc44a61bb5c09c574bc817f83c915af6a98ca142e156850b0ce2d3e2f058ed41263f98ce5317f7b1b08e9a965ed70f79507ff83d89fbf9bfc4cd1d9281b50" },
                { "fa", "bc7aba720c6ca414b5e0f91d21e9a121b6e4f3499819d2e9a8a445716af0b8126c1c9559bf15192193ecac5580dfebfa09a1fa0a71406a2ebc1e796bd678c4c0" },
                { "ff", "3622656363c76f378bdb1517c3648eb204003e60eb3317bab0d6c360737e69353ceecc99db02bd391595f4bc8acc7263046cbeec422f17561b812b5df7fba8d0" },
                { "fi", "1faf4e8962112fc7fa0b56a7b1de89c086584cd93172c6fb33b8ef0533c4f100298988ae99cdcc88d6f8006bd871cb26ef8ef3433383a8da9fb79aff98f7e26f" },
                { "fr", "ae04bf7a92837c04f814a1ff279f8b1b6dbcfe1dd5ba20ad18ca161827816bd3b8ffb4f6bf033e65e367db06de5e8407ebebd82dd79e70f9b4eeda8df949c4b7" },
                { "fy-NL", "f0b14731b256cf57b64f5fe1218d0017ea2d2bcbfb33f370b36d7e1cbcf8b255b43a66aaefd8d5a150e0be32eb4d84f4221bbc19ec67020efe3fdb8874692088" },
                { "ga-IE", "0899c66e4ac4f7a7001b675128742623891e33120bcdaed476f4cac451957450ee75ebc2a14b76c9ed40c0eac93b2cc79b9f4cbde56bf8e2c2f743e37a99b438" },
                { "gd", "72dd72d7473c534a9f55f4cd3762a901be3d6c66fce1a1dfe7e983fcff2b9c85b7f6df924e5713be76b988d7c8640259d2da21aff4d4aa0ee18b9cc642c9f3e7" },
                { "gl", "cec6dd2a00d6ccc036eb0fb510e3ea01665df87e4472b2f830fcef7af97362c6144b37a0728e0444b54ce0c4bcd5dbd1ac67cceb9554d339517ef19bf4c69c41" },
                { "gn", "5292e264aec79565d7b4062eb380ca14344d96f521b66aff0dcd0e77a99de5341a26ec2a436d3c6935d366c5758e1d69b1659f05083f360a13f60eb2fb338833" },
                { "gu-IN", "1fd6a3f725d74c0f7a1e0fec5aa3707ebac894236038c7c359c861120b9ae38b604404878384bb5cb101708fe7d32968769c8ef67e68bc909e8e9254e5b82c8e" },
                { "he", "e97c12e69a07d44ab3e32fb38c66639d2f3076ee8cc71f34b4790313932651787ba3705e7ebccf6016f7f1b5b04d176e44e5d15766d017abfed26ca5a8838b22" },
                { "hi-IN", "e36c0e7d936d67360cd53b1cfa69587c1643e4879714b915bb06499ce72bea168b667d9d5971ea75c445e0b4d63c32a07a164656765ae0c41b3b7b2f3f540016" },
                { "hr", "1c917d7d43925ac32b2e5de3168ddb573d60ba7b10b5991658c8e2a556a03d6be3c1ad1a3edd4b6710ed9f1b019aaf9106e9f6d9316fdbf80db3b6bb16059ccf" },
                { "hsb", "979bf2c94712c8d1fe16cd0148f7e2f77cf39253d56ad7bf946686bd1f002ed8cbd38a962733301be7023ab73e1e72abca0b24e0d71957bc7910cb3f7bf77399" },
                { "hu", "90f0c2ae2a7b314dc1caca31b549e37bfe4a0bce3d7cddb6638c72240403832d87ddcead42f20be5c6a2eb2a1905895885a1656a69c54a7b74c20e908cd7a8db" },
                { "hy-AM", "9bcb5f516260cd3df2a9cdfea67048a1442e0df73fe0d39eb5a07e5a56735fd0f1fff5f026c5be666c328b1aa3fa2cd17815a931ac82ecd640bb301551c6bb2c" },
                { "ia", "778e7fb5be031c510847b70e66fbce7f7215fa77a19b636425e87ac29d084c11d605168430957ae60b2472618e8d5447e1b41f88282b650390101e742de2f79c" },
                { "id", "86546ffad9b5d844d73cdd4750f40c22dfa7c28d31925c5d5452d08df1b603a674852e58b5ffb898724b1029e5efe8305d113d4cebc2b5f9ffd6ac62813e5f18" },
                { "is", "28f490e03c9f8c08e0a4225480c4cd0684cac2d18cb5d4731212cf08a32b4f30bd1cac62a0e5824c0b52c806e3795e60eb56a15f992be547c1b2e31b3287af73" },
                { "it", "dd46948060b272ee956786b95e8289562fef08e1de003a4a49a10e6edc66b8c0c8eba715d01d15126e4380ccce294107cf83b375199674f7f1e812bbfd2f40f4" },
                { "ja", "070a88091624fb2e6130eefba253b1d00d46546d4245d79f7e7e54240d2cea7d2c58fb7d4a7832131809f4d3e73dcfef24c06a8a8b53138c0b5a5328bcf96423" },
                { "ka", "75a92dd1183a9418067eaa305552af083169c198e696f6bbeaa18cc977b7e580d3b4a17c73735b499aaad6446e4aec5f19f5264fe67a9be5b95be2a775cb3a7c" },
                { "kab", "f1655e08c45028d21901386604bf774a6ef4b21aa0f267ed294ceadebdc5739f08fe589cbd85709573d615a3cd098523da6864d23e62e1ea84893e4904758cba" },
                { "kk", "c4a4397ca0b9697ad671aa0ec0cfb428e0c955f4bf01872973504448790a2b5d852954145ec80d00f42f5d60dacca5820cc3fcc5ab0a67be9666afb17af19e37" },
                { "km", "5db466078e64e3c5995affe53d821b1c65e6062dd0632a465b7586a453200a2a9bd70cc1930cbf3dc7eda1a06c019139d19865f1bd2e1288dca368be913409c8" },
                { "kn", "312e18c80b5937798d209209c1c81c0c386e7c0c16f98fc516835db7214f18c30fb433fc0cf58c303cc4b795300a3af0f2d3aa520e87b21fbad420f353cf57d5" },
                { "ko", "6299df15c3c43e587c3bd8558cf7d315f073109cef9d20f5174ae4e15a6b1994ef749c096590b62e1ee4bc21c647fc86b703e9fb8d3dbd4939d61afa199731bc" },
                { "lij", "ff21a8ed1602136b3a6b71b5106d7dbe2299224ae9c6031f8485bf1bf06d365d4a945d0559acab7009505297b3b948f1b7da0594d5687bcdab4b0e4501f5698c" },
                { "lt", "7ba2dcb2f40f80298477a32fb917fe7096ed948f4bef7116fe46862cc9c1e8dfd1f0ac8b61d071757477763944d807f01635b5c2c40d69f8636a152ab0926ae2" },
                { "lv", "8cffa8288eec3920dd86c06bd5a9aae1784a3eb92379eaedd60dfcaacb121838c725697f4036aba9598e958debb02870b393f66459482abbd90a47f1c47d2dc4" },
                { "mk", "32328bcfd81dc76485624b1256ab08dbc987ca2d2fe7c60aeef694105d3b1c90bb60b97482a58bb8f401fabb101d0ea125edaad3bee09508b6ea7aa6519cc379" },
                { "mr", "95919ff513ffb0282303f957712b757629d87e67c6078b827715dea7229e0331409f98ba164cf8d3b004b6b0ed2c6ece0ba24138bf10ec2637d3abed526db867" },
                { "ms", "23aaba89047d0382de0dcc7c776b0b363607da0b8e6ab9f0dad97aba52300ccf477ae29ac8fa9d6410bf9adffdf909d497c6b8a7e2f87b56443fd84b7e8e0d12" },
                { "my", "3270953dbd27b3337e1f0856163cd55ac5faa625d5ef094a3f473209a1598a204001232ee9da2209da9480562bc8321c2e4a33512d7017cd33798e38830f3acc" },
                { "nb-NO", "95b367ab6d9742a60934e93a0c8934861ade317dd2475f457fc1de693cb7ba2e4da0b0cfd1cdbc7668505eb3a901ef89e2694db9465cf86f3f9967cdc1835f70" },
                { "ne-NP", "a27ec8d41fbada51d8669dd0fe3040153b6d000645e04392190e88b6a849e4a575c06f64b5fa7480a68af7400da38aa4402ff5b300519cc9b02cba5f49132c74" },
                { "nl", "26df7837d6ba25a148591e176de50cf254492804c39be2edcb1b00c98cbc82ed4152ab46869d7fb191ecb879b544c9629ed4f1c06bfa3c382249b16f0ff878cf" },
                { "nn-NO", "0fc850042893aa6d1116a6fda521ae2a60f60e0fe41e830f6daf26b9142e9c5a7fa9938b6cc61dc65a54fbdb4813dc233101ffdb30ba3295906c3a91141c9667" },
                { "oc", "7bc510e47cb1a5af4e763fe3b0d80fe7c8634cef809980b99e317f99fa3a134c0fd15c17fc7f6a62f4662fe25172e110bd39938e8a8906bc38c17a0036e1aa44" },
                { "pa-IN", "ab00115f47b338671ffeae9229a4d9ff600d1846d805fe1aa4bd81a89de681539911a276eea7837a6fbe36616327feca9e71654ffc1e970304cdc2325297b584" },
                { "pl", "2a933fd0308bdd9d13b18a7f31c93567b231a41637c3a89e64d2936dce45e2e7ccbfe6ac161269fc5d2204854384db7cc716ecce621ea4de8903de49fb433a33" },
                { "pt-BR", "42de577f04d646084e861431e202bda421c24071b3a802997ad08b34b060cbb02c123abe92c5f36c6430b8e83fe5e9d5599a4da7151177d42762bdf1113e88e5" },
                { "pt-PT", "6448ddd0e84de29b3348aecf782edcd5fd9b2a939a7df47fc0310b3cee6a58cf399f7b00075654d200a8eecfc416750fce9df69d33e12642df72ef6590580256" },
                { "rm", "cb6874b70add89d903de29e35f906ffc5db37d2f22cd7d019f4bf994cdedc336fe645847528e85bfb70aba2c71f6884e8413242d4e2ea654e36eb0e2fd6450fc" },
                { "ro", "57420ee3d33d9780469ea567ecb166c349c64cc80b22f39a0dfaf1ae66389767d1223912953f60baef2f998c7b892ce98636d79caa12f93e8f6d10f58f95ec01" },
                { "ru", "22ae89126b57ee0f5540bb9dcc180018a2d4064ece422079ab45fae3f9f8fc263e227084bd8f7d4b0f04b4596e11d207e7cf4e7498c26d739da42a67a7630ed8" },
                { "sco", "a26f884bf2d298a5d545a55bd008e7b443fa34b7de79613920dde725dc4a637e782b0f8d40b77d66f6b639307c81c156b356810fb5c18f6a0f42e503afb7d8b5" },
                { "si", "4a87a9fd688b84eae9c52390d09b25ebc2ef0b6b8674ae114daae066cbf825f3fad78b37220fa8c2b74cb49ca93d183a963bbb8c2b1ed6ef13d39fc35290afc5" },
                { "sk", "957dc677b91fdeac68968e6fa8dff8fe90a28335e5c1e74fa6819125ca675c59732c4aeec8ce0334d91364f38e8f59c433360c5355d5ae74bd4c769d1c3bc7b7" },
                { "sl", "20c5545882e0189bd196407cecfe6373782d851f17824c2a808a2cb37daeaa5a3cc668fc9fef05e2ca2d854eb2fc28dd176ed3cc6533b016df44f8510ca46190" },
                { "son", "90bea6d3b7c7c8a806eaae273c46887a90198bc1961d9f3c03907be438102a2ba84ee5f52303d4714a6ffa4439eec6d057f1106e325b4b22893994c00d5ee977" },
                { "sq", "b5a9ae38b0a2b661f360aa8f44172f93018e349a48b45add603a0b4f4bc8f0338bfabaa99a092e397dcde8d846b019e2c8c52f860ddbd3113719e53108393e5b" },
                { "sr", "44306b1dcde4976d46ed390ed2881e08e8973e6d42666192f301a05fa90434d15e6afd5f6644de59e8845b8aca34febbdb09ae0f61cc4ff6e22e8a12f6cea11b" },
                { "sv-SE", "68ec332404e7131a73ca29b51317963afe26d84378b7d3193dd0782e53b7f6ca70851c2aedef2d76c52c2e9bf061ddacb4b1936b17317222918d307ed9eb67b3" },
                { "szl", "cec07d163398e092e7ed9f428a95ceffe394761194ad9518a92a08c4d1fce2992a23d4075cd8630420947c23f22f4d77672fe037fd99a8fcecbd3ce0cb9d98a0" },
                { "ta", "c65ed5f23e6de50e6ee35dfa03fb95ecd2f6d066ac5966ee9248a81e43bff063481a27940a299e3c90f938ee95be2a5728eba40259861630b2829f982a0764ff" },
                { "te", "8cc4579fec3d22c857bd4267b67d7abb321e1b9c07fae0aa87a765d2138b058bcaeaf0b5a183a6b1957a4577f95914fbbcd816eafc442d78c43daf8c9a50213e" },
                { "th", "8d04db175cebaeb572ed9f391a9820130d19dc19939de2d41710acbca19920d20fe204d9ff494f1cbf8c1bd054a95debefff3b0bd04d1ea29f92ab32e1911816" },
                { "tl", "1c762281c7610ed28d4fbd825d05663b695ef7b96fc94d4b2e269f2637628e56aae23324d67cd15aaf74200d64ef50cf0f1719a349dfadb71623b214307fb3da" },
                { "tr", "913424c18724faaaced56682d3cef5ce7978b33436833a412f310107cb2aae81bb1c54644a38abcf0c868cf290b4a135a4c9d3474ba3adca4db76c470d95dc75" },
                { "trs", "ac831ef888723e6653dfd405a361d5a23c2fdde46c0bb196301b94fa4bf46a078f5b1c6af8328142e0b00af8f8a55913d961c8331f7bb58ad03e8fc0d45bbf5a" },
                { "uk", "029d631ed411c3b1b9b73ce26af8dbe6d2682da0b07d1c1c2bf454cebe7e4496b525b061b5ca2274919519a81f6304aa5b6b1a9d172e6987975548d9918a4c43" },
                { "ur", "5f3582704b5a828ce701c8b6ddcfcd3c5ebe66cd1dbc0cda7132c122f82b6930a02f37617221ce98ef74d55029920079a2d7260d4e2052db1baf2573494c90bb" },
                { "uz", "5d119871e0b22670463cdb4731117e93fa48d86c76d033156e17f1ab4a534d18f2d74357bd9b078a32f4155221bb3610a52f4df6bcdcf549c6fe4c549a0e1264" },
                { "vi", "9059d7ab36ffacb063f8431d815710f89a0c498538df47f0034b202eb0419688b55a21ccfc9bd2459aad19926d947773fffbe94f23d734c7bf4fbadc1b9574e2" },
                { "xh", "c67b3be5b177864a481a617f6a880b724926b2b9c0768dbe2cc47095b4a83fa739b2d44e1ded9c3105ed1682b14404c539601e64f9c1dd05eb89847d3b51ba92" },
                { "zh-CN", "4683b6cb84460b4880165c822748f3952b754b6f0cdda10af3e5dfb7316468e152cb2cd4f1fe3fc9321f7e5d0c7fa45011b1613d4dbb742861332388392efb12" },
                { "zh-TW", "a6ae4852c9c470087ff24d8881a8d0a7c1c2fc4ee7429bf94cb3e5050474dfcdb28f82f74ed182b32ebc1d0133378575f7798f28186ec356adae89be995e4dab" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/105.0.2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "cbdbe4289b541cd43af1b97c90a014051189be81a9767a434e95d9ece783449bee2cfc946ce219177974c0fd501b6dc2d05d468e56649a3ded6fa9e60d3cedfc" },
                { "af", "0c92e1b1bc48108adc80342f0ebfcddf76a99fbdbd327576c40af10b6e8649b54d046a5723d163d8904f312883fe7879cebd92377ab49a22ebcaea7b16e7bdc7" },
                { "an", "e9e727af5a826d1abc82ddb6ac76764077c95837443b588c00ff0945ec36c0313c25000e18365d3686830391e7de540cfe29e0a78a9028bd5aaaae79ac215bcd" },
                { "ar", "9799e944dbe41fddc906a017e5e1438df2ed652483ace2338ac171694f7a82c2f38783690fb3d61d3978a876b3f8e58a3d143c3e5735736c933ec6f2d79d9ac2" },
                { "ast", "768bc09a31cc8d95340248b3129f6f0ebb533dfb27a09cc633908284eb0e9e29bbce1da417f7207cedc99fe047c5dcf42be6b03936602ea53935ea7cdf0a2774" },
                { "az", "a98e18a9c9085a2c4fc853e3ea93cb61baeee508cb392219df8fe76b14e08e7740a6f4b37ea188f24258fe5efd0ebe09dbd5e97439ddd99212128fb8fdda25f4" },
                { "be", "cfa2ce9d74b5a3b18acdddf984bcb4ab3e206acf91f3188bbbed1e9d004511c90c4096e77626476f54f846b6daa6e9a108ee43e334db4d44eae0827f4c944d2c" },
                { "bg", "4600d27f9b11def58947d6c8d2f7fb523437f2f702c9227def8ee5f0544ae1a1f54b49cee9b68b03e4e398366469a14ef126a02bd6febefd96f0d8e5bde34522" },
                { "bn", "38f82ac0957869807e8571cf17a081396c218e1f2b84066759a59b746bda9d4624b27b5ea9babe7a6474784707167b27f72921f4055849599e2830a6c106dcb8" },
                { "br", "6e8484d503e435d4a714ae7f86e7600fab816e812ad0198fbe05add340a336781df33158e988e6f01a5473e3e0da16517845260fc4f13af87ecfedd3dc7c3352" },
                { "bs", "016966ad4b223846c48c834bf8a0f1b60b04dc310bda8933b0bddc3d735f9aeaafa6f7fbedd84b049a55adadac3c9791417cc846db7ae88b22b116cc3b8e084b" },
                { "ca", "b2d6f4b6f146190f1f2cee184a6077f92f6210cc7794a3ab1df887fd7b44d9f0740fa3a92f180ad23c1a1a513f5421524a42171a78eb65194a8b08541f829c44" },
                { "cak", "2b8d039eeef0c876d5619918ce4ab9fd14fe162ab114338550e4cda9a46e9d8897417600fcac15f4914363c61d069d75ec976d23d6802da544e4458d88d3343c" },
                { "cs", "d5c9f53f71a4b8c30f05a929fca3cac779a24391fb3f5d90c5696ae66a46320a2d3e657c4128fad3950c84a8564163da1b785d0e2f9e259fddfbcb7db485cce6" },
                { "cy", "5de3e5f13a33a7f46dd2ab6e29a07b0cb50e228da5bc07b9a05149eaf653a1e244ca01527e061787c85dca595075c10f7714751ff892ed6b4cf4d98dd4943d38" },
                { "da", "1f973eef45feefe205107b6418c0f7ec5b9c366b48c5fa138d765afe4d2901b7b671130980cb9683291787fee71a8b0b96c068d4d55939aa28516fb9cca6a6cf" },
                { "de", "697aaef57a5efc82b12c59ace370798ab289709ba4b6d46ef456113381f4d2005f3ce4a0edb002dec4df7460c7f5abe89f739d865148d122f1aed10735ca0c83" },
                { "dsb", "aa4769160d7caab498e0f65dd463618c93750341276eade0dc170df6ca5ad6ae8ed7cc0227551781e43ce09efb957eb116976be8435775d6f7f37ea0d34a5c6e" },
                { "el", "9313539f0b5ce65c31537907a0fcc6fa7295654879df068cf462b58f7f796767bc2251c2b07e44d5d7d160b7bf7afcb54afee2f02c5284c434db29fdb7ec33cc" },
                { "en-CA", "fb9a0820dd23d228a8a05bca09e6480da8e73b8cf06c58f6004bedc5037f287ed2aba4cf0c6c42fb356786a7d601bc7c7af0b5e86be072ebffb97eda75d6b49a" },
                { "en-GB", "ff1f7c1c199f73f51e0f645d2e4abb4bb08f5f9aa822f7a40e52b7ca462fcb268d111a36ff9c62cc9f1819bcfc3a66f4093fb5db417d54bd717eae47c7b6ae7d" },
                { "en-US", "256d41239b8909add13680ec40e288c24b625c53378dccd8a644bc68b08bab0e58e4ddffe1e6dc5420877f434970a6a1782e72bcd63150253f9fac8b2c683941" },
                { "eo", "2151a3615532c6f5db5ed601fa6d09d62344b6faecffe74217661e39322b64e2468578e4b88bbf07a0378aa57c4ea22b02d5413b2750307db3aa7682c6784bf2" },
                { "es-AR", "18c7a8387c97fe3c32cb11ef033ba15a212f0b8b5c9274e4e2fb996ad017f19a0d42f5ce3492eb4a73cb1b92ae6d2c19d296e357d859212c0a93399d13c8f35e" },
                { "es-CL", "51eb8efd2c973f4dac1ba883b0f1f03c7bc81cc731e56c5ece6fac8592b1403ad59dfd9c37ec517c49fd230b11f378c58e085768ebf2a6f1c51e5055a2168782" },
                { "es-ES", "bfa9194c6b15f25b8b86a1ddaeca265f440ff07b4dcd92ef0a9b4efc15d9674ba7c7a67e23ef47b6399a03b83173c9d8d9699168eab02ce47998b6cb5afcad93" },
                { "es-MX", "d2742d965049bd37f77fe80605cd0c6698f57a412689f8323dd4c5fbc3697555f2d11393f6d6a2f15fe7f6ce547f80695d55491d183a451ea1425622027c07e0" },
                { "et", "63acb73a425cda999f9d40d8136ff669fe34e8a40327c381c00d57148b8a0d2a875ea762ce9528afd4c758b38f7eb648ee8e44d34c10b8ccd4a5e7df20994be7" },
                { "eu", "7245fe7d8490e5a6ae315fb21a5c8dfafcbff0fe9e7d2fcb31cb1cda262af25d303e1810bdb861e118f926cc711f8a6270658a1f02382e8a0a3c05f2ee79ac7d" },
                { "fa", "26ef82f951bd24371b590b7695dc0eadbce41b247c781d75048eebccd135cf9f1a5485447d0f9f645baa97bfd63a4aa02620b2eacd6c839210fb39a01937b0ec" },
                { "ff", "57b625e3fb370ff7c8bb06851216c6dbb1079d9323534420f80c9b6185e8d74fc8872b28697540ac47c297244844c4caf23e81e484fe83a1e94e41d0d8637ca3" },
                { "fi", "c36ad06d32b857f47d92f2c5adaa909987e2a29efb8b7651010d7f20bd65bfa89ecf0d9e69af2083b2f9e6537cac127a304c0d53556b8a91ab7aee24ccf98a10" },
                { "fr", "e64847010213c76bfa83173b7b4f26bb1cdc145505153a2eb4da5595f758398e04b3f60855f9302eed479bd1611f8d2f0d8da6820989443d7818b2b43cfaa791" },
                { "fy-NL", "2dbce0478bf17e48685bab1c7fe3df5444ce691b7cab518356913eef80499c0ece3fb81d3af57f6315924ccd5f354d3f248d2da674b47afac1d3d0c87cd2cf47" },
                { "ga-IE", "9897a3a40cf021f702feb3b4655a6995559d288357df86b962f00eb305acbe8c4fddc0063e894ac86eaaba4b4483c35bb95afd148580ada75f7ff75b4da75532" },
                { "gd", "f8eeab9651531259231d54108ce0281c4905f06fec657fac21ddefe16718e3f037ae2fb89b9b977a392f440876a9116b2984fae66a9e12f1f9a1f41749c4725b" },
                { "gl", "24d3bb3333d93fedf95f542d46e0ed6657cd782154c1316201c11d69a6a950a71f9fb21de54b4495ec55b3ae9bd477a376d9a86bfa99d771a7af02ca4d32e974" },
                { "gn", "c33c8ff964a43af9a771dc6bfc998bcfaa4080c4ca1ead451137c512b1df3d9b4bad37bab5a74a8d6057ae90785bbba6eb9abb3f8515550621557f9d037d6003" },
                { "gu-IN", "35b1b59bb9c858d33b1d08275c5e0a68dde87142d59b6e16623376b8ff7e9fb7e88c66df15b34eed9259736f9c518a03bef7984ae5c79a983a32547a5cea20be" },
                { "he", "021b9fd5be4f6fe503dafc9126ccb6f931308401e634d741f108431c51eb4bb2549dd512c73364ee30b3f62fbb40e5637ad8994f621c045846f11f419834b8f7" },
                { "hi-IN", "8b0ea3e955c659fd1b559931c6f2d29df4f3a947f9a8d9994fdfe406e4445e0beebe086f98e5be0a7c4fb220c9d67d1564dc97fdb22adc74d993ed6b8947ce84" },
                { "hr", "eb98e8d2e1c6d675814436779b2c84b65973bcea0d9b3f2aa50b7b76d6589498e100342a707c8cc05d9c599bdadd6ea717b3ddaea0f007285efaddb6cf1fe127" },
                { "hsb", "e9f148f6fceeadeb340fc02eee6ae92f46ad980524055ddde2d5347f9985cb097151f6f61faacfeaecfa4717e9960961036d75a47bf9352b7df87a018d16da02" },
                { "hu", "efa354ecde879a9d89b92c95d32e7dcd9d5d2953e8edfd54b0db83b3645b39681364503dd6e077121176016817a45fbc9f82c0424d22c58d67dffdf8c638b486" },
                { "hy-AM", "76bf3e9ec51b2177df800c51b096a17d94e6b83bdf516a7474a892f6f89283356d070a98d8f7b865fbcaf7162ad05320d7897ef0aa0615efdd54bdc7b1c65741" },
                { "ia", "4a47865bf42225507883b4237ba752f0bd6d9694c4b721f9a126f7e0e78d46fb0f51b4bcaee11eaca8dbc852c0837c5d656743431b8794317a5364d4cfb32c24" },
                { "id", "31585e6df854817e940f1660e14789affa41eb690011bc208c872aab61eb6ee5bdd22cedad7fa6b67cdf71b055f917d54a907a0399aa1a1f5bd19e93c2642122" },
                { "is", "838f4b69144e96a8c79dd9b25cd54fa65f6c182931a792b6f5aa1897acd1212b31d4a4ffdbd1f3acbfd625154c3dfa5e2cdab0940de9b38478ac0da5f684e445" },
                { "it", "a35c17f58cd7876a519afa708ff1f415a155467189f61dcefba5577d04144b5fd613cf65f5ad32b69036d392e0c5d891307a565ce85e2f5ab269ae002b0472f3" },
                { "ja", "ffbc72e2258f6ccc4ba8e39d9f2b18f30786d24717176d1c713a32c612e7f92b90a3f112e993db43ed5569793ca7209aece6d16153d1d5087995aec2247b4f54" },
                { "ka", "a7353494285afe9c96f096c0f71069ef56e5d3fb2892a4b48b63c0ac89b60d887162495024d7402d9f2ba30c5ae9b390121841fd0be2dba5eeb952c0f215b52f" },
                { "kab", "a4764a4cbe2a6871f6b6d8179d98e360ad4c8be0686c9ad703b627fd225e4e26c11d4c3f7871ef955d2afce300c56f459d336ee51e673b2c9e0618b054276428" },
                { "kk", "4b601858e369c9f8a2759579d178cd91e83a7cf9355268601302e83a0fd6da5a1365e59119d64bfb38efef238abcbdd2f5732e82ac2fb71075dbbe747fb1020e" },
                { "km", "9a1186098f1a3b3c644cf2be4e7f78ccf352439b62daa4fb838c0fcaf210ad81620c57a26147cdbc666028e5ea0e50559efba65132d378e66c2b7692e05e6566" },
                { "kn", "0d4979ec91589e1232b8a0630c47e791c0312737799ddb89100d274d4878ecb2726c250c90a7a271ec93b32136d7cecc0e9f1ab1574d2ddc798e57a5511978ee" },
                { "ko", "8ddb4e0019fe74e3f11cffbc474151a0876db5233201a250e5c46b717a3aa32c948416db0e91b60cec2e8323f1d925449bef4c299c60c8d90c272df05e03d3e2" },
                { "lij", "ae86c95723f7379d2bb48611ae14ad1aac4e38ac498fac80e4fdce9a9e5b86d5e491a19048e879c362bad6a28f2e4f498367523ede5e77a786eb39e782ac2cd1" },
                { "lt", "5f7bf816c85cf52a820d5a74f013294aa6080d2ce1373c2548d5ee0b13eaf08a4b2d9d02d5d2c465dbb499d205cc5bd8c2761498cfe097fa9476110b377663c7" },
                { "lv", "118a654425f31b4f2458e2e9a482b72bdb50526d5f8da1fc6423c30969fad99137fefc54660295e0f5bedd6ead4262d02ac727d4037b5b6db46996aa4bceb3e8" },
                { "mk", "111d9112f38e5704799395da9284704944dd2846e833ba5fc21b5fa7bc9ca2b6a7bbe76d68d048f6f29f087ea9ffe6be45ac89629e7c1b24c0b50796d596636f" },
                { "mr", "9687738ffc3682a28596ac7fe7a9d1ddb7ea997b55b8cdeb9fdbba03207e74298382ddef71efbbfc3912e27cbb918da7aec04eadb4993aed83e45ba509ee90b9" },
                { "ms", "add247a87b36c672a6dbb3dde71a9c5de5377cd9e71151a8e6665136bd606380c0f0ee98419babb985881065a424a5f5d6d3ab8ba6033bd2e680b0a2cc1d6814" },
                { "my", "227af92fa6c6d6c650cc5bd15d4918d55e22f15874585f0e3d413613ef320690127af98aa35e3d4ca2fff644775cca89b2ee689555838cd0f7b6484da0d928c6" },
                { "nb-NO", "9f6b8a4e14220af42feaa13ff16a1199e279bb1ace65f66fb64f9f273022159aba21a46c602b7bfcdfb12e7e205d3ee0008e014c4c6bf3fae56f67afeb04a8b3" },
                { "ne-NP", "f80af40a46851779cda9dac63df6250778fd748f13ef6be774d1f4d61d1d3e3cc9c7f965f24fb5f7e33c12534649ca575706ab4a1da1df05359fca0398e12e13" },
                { "nl", "ecd926a704f9bd7fd69105854c7ef2bbe0bd477bf3a4a104a95177893dc36dc8e0b0edf30ba3e03fbf031f2f9dcc447bb25e1ecb12afa1d2dc959af1518ffeab" },
                { "nn-NO", "d8378eecef622cfee3e0a8cfa82dd3616fda032ff119dc2aa9164ea506c084e227ed86be1bea7e8531d3ecba74c2e5e9343851fb6b02d84ed2a52c883eacd2d1" },
                { "oc", "aa7a8f5e378dd63cd8384ccb7f8e28ec81697aa9792abd3829128e572c3edbf42108b38ae7f45c15401a3b10cd97248350cc010d210250caa38d398157cbca18" },
                { "pa-IN", "5f6db9fdc30e7a5f3fe174117c9ce806532e287495c001d9f571a30058aa182d1cfeb2875d8134be23c067eb09afeeb6160215bc7ed07aef8b68820a022247c4" },
                { "pl", "f178371b97a9d647bf0cdf95d0117b0d93ba1097cbe09bd024abfd9cebedba61785e767c35bfb4d4b35886d57f25462205696d2c1a63ec9ff080b153f568adbb" },
                { "pt-BR", "9cd1b754c9b1819f7ddba2fa00831c692bc2effbae05c153770273645711c5b2a4418cbd9bccda4ac2c33fd6dfa85d1fde523eae15faa77e13daa9ea08dd9fa0" },
                { "pt-PT", "2800ee340d6f2dd7c25bec8f631c48954be771db88da8e20fa391a18d4381e359109eda47e2505e2f9ff9b950135ee8b64eb2e94838f97e7b75252a1bd33b73c" },
                { "rm", "f0b0c019c1d15dbea602adc9a7af1f967b6b0a42606ac6f8343f2dd33ebd90bb810128b7ea5abfd34291ba00b00b059e3373dd240c8aa69d5c29a03db0257ea3" },
                { "ro", "5f107f003dbfb53a482d010cd5170a4c420485f2c52ebd28e8b6bb40a04f5b96ef9d3a532609cb1a8a26b94337b0a9eb77b880b142896e5eabc0fb474e6e9132" },
                { "ru", "1f326dea18dde316312e017622d7c56f4c46321ffaefb327184cec705965c83dbfaa65bf97ba497047f528ccef40115adc79943a7b6b3310f6ab780df44d9cef" },
                { "sco", "427de6d7760fa9429e382cc009f74b9cfd9ba299fae33d099cd0bc18d74abf861dbd0e7656724811d926718b16f292271be8e577cb8b3e7bc83d8dd5ed146e18" },
                { "si", "490635f68bfeee4524edec25e0ab34611087a8fa9f3aea5518959f57f4dda86964682a6208fb171ca946ac9b9db7e060fcf69a90b8d944781947542ab15fe40c" },
                { "sk", "8f23856e7bc53737775d606d917db8a295c5a4d4c7d464ae4b89691d9f9e004027a108a3583d455ff235180c50298e469b5cfe811830c8e9af3e1b47403c1594" },
                { "sl", "7124a93726e0f116cd16e7c14432e5d522888d5684ccb9ead34ef4ca8e5a8e46c6a21209ad3184acc9df0525306143f7a9b0eb287f269b76ede4fd396102623e" },
                { "son", "48f520a71cfffbb4dd5bc4aea62b1c504dcb09c73da18df6859d1975e73d14b62aa9d32e00e43cf8ecf6ac9524e87585dfe857dda1e87638655169a974ca2eb7" },
                { "sq", "c16def9d1c90585fca35e12f3e6b5be382d84f94dbf83f17ebf77e6ecdb881d823b6a1c98b66d7716ce4a18526328365449b65606dbe0c113f11aa220c35688b" },
                { "sr", "f9c3abe948166e9e9a51ba21da3923ad3faea48ca1ca6d67574a4b7866ccd6fb521c144cf6057cfbce5375dca549e92b38d5a39fa9376a613219f0620c58a866" },
                { "sv-SE", "a8bd14cf9058ba68c5ac59ce80c06a6b9ee0c3b8eb1dbdb0ba61d409b2f4246f279b733ea88e0c9bc61cfbbba3cf1948757785a44d31f5dc6c4c2ff87a138adf" },
                { "szl", "bab6d76a551a65339d084e0652e7e0177e8c058b1104c49b0d22140c0af1fd4c57a5b14998507ace651344497c951a5df3d08e7b333fcf1f6c172608de97cede" },
                { "ta", "fc522579fc481cc712523f062c1d46996c6a2d0f90047e437374395497339a9340c04a70ec349346cf23a5f8e9beca0a356abc29144e3d7086952e0bbdbf8347" },
                { "te", "5ce1494374e3155e32aeab7ef905a3686fd3dfc1c2d86152ec748867c0c951b8e8b3514433e093c3c261bdf74c7f0bf63f50946559ba496d3505ebd1bd4d1ccd" },
                { "th", "61718a0d22f1c807a50a59b9d49913bdc43f8ea2df5450d05705be4894062db470bd66d870b099ad9b4531be6c645260895dbb1ad64457049883647d12538fe4" },
                { "tl", "5308622f430e3d19655b06690ae3259648fe903bc01c2713f48722b0a8337529c33af2b939a32cf3a30d0122c68e561c4e60eb83dc1ff41257bad943ab0dddeb" },
                { "tr", "5784db48bf98606cc69cea0704c9d52064f1e997046bb016e25d3b01b0f6af2d0a48baa97c7e9db493b16af09efd6ca3a721a1f4fc14743a3bb6f22b66a4fd53" },
                { "trs", "c6830cf4f0ede730114a36e28c7a8ae83a0addcfe1dcac5b7deda6541855b9df86ff452664273094aed5954e2d17dd437f05331b4c06b057accef729af41c480" },
                { "uk", "18d378370221e84c21e0d6bf7ea90572fc6c8f9ac14670d5c5e23b59b98ee4f268808d12401fafd945557a4781f7c77831f9885a3276292a026f6b32d6d58369" },
                { "ur", "7fb0edaa8a2bbdd8ee7987be4c6ec508a566595006723b44a6e0c76b47d30ffdf1326a4af466b28beb58241aa7fe86a4563bd6e5ee4cf126f8ded2d87c39bd84" },
                { "uz", "42db8ed2fb8d92f3780f65a67841fc0c687c5d3764037394aa2feb57f5085244603df954543fd94f53742cf519341c4c7093fa237c7a7697c7e3aa27051805ff" },
                { "vi", "80403cbfc64a7bec37e2c468da2e9e61e04fadad198dce37e8475a247223c652135364b043837baa76b01ff6647b5d7fc1d1374dc7f53ed15e156e71e44af6e8" },
                { "xh", "2f941ec088d22eb584a3b45781c255e7adc18c170e60d024a02ab25e4fcd036f1f7e2c8773fb276840f267c39feb8d3882fa7e6988442e80107064d38ff59263" },
                { "zh-CN", "5c9edc2d0b66ea9882ac101aee50149be03a854a02bcf86b8348f4e57374b74e8e67d8c86746d2f76b09c7b4e2c82f8ff0e7e9d59ef01310c877215cf4683236" },
                { "zh-TW", "183cf7197488f50493a3f9337d9d7fdf3341caf8d2ab59194d5dfd20edfe34c06c94d507c5a0ef822b19e247475f59d9edc1d803dfd384a59ec39d83761f425f" }
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
            const string knownVersion = "105.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
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
            logger.Info("Searcing for newer version of Firefox...");
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
