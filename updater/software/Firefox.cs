﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/137.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "eea2df7205d5a0ce5cf64b8957dc6fb98ea250805b147d30446852100e0c62bf268587145df5ca85db29c34a3b44d5576120ab7d5389d7d274e55bb72fac7f52" },
                { "af", "e2d08f50d2829eb5bf23f2fcf811cf8819364bf94a0f72dc37be70ca83faefe3d5da6e3a14e28bb0549ff64f6eb866a570883d60c8cffe128fe485d3852e036b" },
                { "an", "c67f591afec9daa95e03729a731fbe2bfa69f62470e6d226308bc013a737abbeedb0560af1b72bdfed1c4800532b4be85b676c5938df76be72baaaef86068bdc" },
                { "ar", "b5fd0dd17ce47b7e5dfbe345b2c72b742bf55937bda7467b239f02e5d67ea2c197dbaf85692b035c9f396d7afc225755afe5d1f074f7a39f36c52fa235fbfd66" },
                { "ast", "3713f9105fdd6bcdc868d6ec5f78cb16c47aa56b813a78aadd57b0e4100227702450ccbd5e6ba62fa3be5ae67a5e4e611f7b109d73e2a8e0b0069f1708fc412f" },
                { "az", "ba7bfa18ebc34b08df44898e7d9847659515fa73d24a7bfb5d98eab5214909d308c3899d86992d6740826a1bb6902cc3dcfc274d7f3bd6adcbb90e541b9ca999" },
                { "be", "3015cf7640e41a7cd13c8b9243fbee3b4df85b941e3818218dd657ca8ef03ec7b5b05ec1ff3b3b3868cfc24a0d02763f5aeb21fa85e393d91fa7d48f2eba9193" },
                { "bg", "676b034d973fbe6ede206d9092fcdf70c045fe2d3aa1f707756ac32564d4af8539895c5fab62b577a4ab7e761c1553e2109127b0b71bb8c1b9bbd01eb46ded33" },
                { "bn", "a58c1cbac1b3bb94c889b2888494003100b8d07811184fe4bee884d098300983d196651ea22ddffb31d1d4098ada5b54e183dd818da32fe3af2a5ba188902a2f" },
                { "br", "e1a9e5cf21898306f3797f8ae92aa765624570eaf3aa342845e787d81bcd8041cda0096fcdf7e118238cf70a76bf28c830ea812ef5d211a3a279bdc1fdbb0077" },
                { "bs", "ba588785784fc3c0d89b9b1f1ed76d19974fc91e40a0104de51d2ea71fe6b247d51c2bbe3ddfe995919f393ef739c3caa29577b89acfdd804a0ae4eda01cdfbf" },
                { "ca", "69b3428a7c13439dc684866aef4c6895bded1a428b030f9dba145dba24a70de6cc36befb7c5ea239e24aea8d93146cbd1f713174bd3cd6cb1f9e15e22381287a" },
                { "cak", "87fc3bacee2b3231efee6e761644de3f62a38a8e661a816e69de97b446f47d65c9d014c9e798b2008bacc044272faaa1930dcfe28fd188ad2704b1fceac989f9" },
                { "cs", "998ab6ef0759a52d8af57a65c509e9b5b43135adf44b64aef88c7dd6560b127613deb81c91aaa695fedf4efbe7a12796c0016df911dfc2a3d446ffe9bb6fa889" },
                { "cy", "877e86f47596169575a0b7a1258ac752d9238ffd7c61318ac2024d023cf651ca0d87c7ed1d70c8693821f2ff5b8d234cd8a852ebe3a1b3ded6275b8d14bdc3ff" },
                { "da", "9bf41817159959b639a7b5cd84b066165a5ba9104648b5c8208b8dd2806f6de893074da4f2c1ae18ddc8f3ddc061ce2a34044b75ac61bfbe18c199bba862c7de" },
                { "de", "af205948a0e1621c1e986b985aea11ca78b0a4f618bc9a72ea51fdf930ccb97e6004838641cf69068422f59801142abc3681d82f40c3c3f8164b7c060eac9f11" },
                { "dsb", "275ec8e9b83897fc9060c94eed9cd96182a7c1b9cf143fef9d0c051e9344a02b7dd68ce9dbf1a2cd0f1fbfe0ed26fa388a8915f5908410b7adb4af09621b87af" },
                { "el", "8aaf1e942f941d544515c177165d0edfde61d3b1b6f35835f8928222dc1dac24a6107d27cc975cd7aad62f6b13993bb4eb9de1dc34ea781018e049b6585cffa0" },
                { "en-CA", "002e859ebbd509458c1c1df01fe2a893d11892bc6fa26234d55174ad94c8889a18a9224a1260ce06f177a09de065473ac0462d8aaed3026411aae6f2bc6807bd" },
                { "en-GB", "806669d1d02a67d54af476494abed778ac5caff660380e1e2541ed548cc464ce76a4a2cce5fc651b88652dfa7745a360cde8581d33055ce55a2890f373cbbdf5" },
                { "en-US", "9eeeb15b0820aa0b470886d172f46e08973ee81f656ba19c58843c6f602837db3746fa50f1ada927169b973a6c670ee508707758f8806ba2da9efec577bc1a2f" },
                { "eo", "55c469a7131667d3fecd3e6354c5562d96521bfa2a4f61d6007bffb0a7d4f841d4c81934f2805f6d555834cf4bd9cfa3c1dfab38649842405b42cdd292215354" },
                { "es-AR", "76493a41aa03cbc2f995f42f8709ecae3b222bf5666c433fcbdc4cf6297c9bd0e169c97a996e451b4372f3ead5d9c52ffe0f2a8504785a7310e38a0b4cf963b3" },
                { "es-CL", "60364f79929ecfc65943693501f8f39bb4b2ee3cb708b21ff52689990fc13c764eb224c00158751fb8d00e10183b49f29f5e514f703692afd2373dbd0793ce51" },
                { "es-ES", "4da759a0b5b0138ba87a1cd032d41fba883faad012304bb204436fb13e2da43c1992da37e943670ba1e0dead1137c87894adaa3eb490eea596b9a709f4b01aad" },
                { "es-MX", "3924378495a985d146b095926635c2bdf8fa5b886a330a15d9cfaf484b25423737ac57b00423409d7df521d684df6cdaa8e7d91686fc0300d4e582de4bfed2ff" },
                { "et", "95b4ed9831290a59cef7145fe76d902eec52914d333f510a8363835f24ee3939f2aae1e36b9bc869c9212a1b04471535f2b652eecc4b31d6219b36f301a5df9c" },
                { "eu", "a692b1e6aea328a9b55e4748ff153b5ee75137622ea69e211b9dffa2b4cecae99ebc224eccf61b72e63c193632a1b9f3f0bfad2c0288d2fb8da85d0b5a09ffc6" },
                { "fa", "5eedde74b60029a3c89eb4f6f04b7090ca342a67e24493e6b7c62045ed5bee54427bdcf5e39453f397f152ad276a6a58a5cb9dee6bcc2daa6879c4b3a27bd1c3" },
                { "ff", "1f07966360d8e231c465db9a7a907bbdb266004cd3caa3e60e9a531b1e1ed70273fce974a836995fb1ace4a505e49db9bb5773b952197a7c34eaaf9c406e66ae" },
                { "fi", "ec3d91b66739ea320d35c360e9054a4652c3a67afc389232f3db5afc584f6633fca3ebf61f1186d5e6299508c287431ede8e342d114782ef5607c772a071e36a" },
                { "fr", "3275916169196fa0117e67fdb9cf6e965b1b124acd8606271a9f83218740a8e0c847ade8a40d1d6ad5532fb9dc78a93ed16aa1e4ba8f67f41beb0d69b9f49c38" },
                { "fur", "9267225ff9c2f6746df5f3f33af8375fe7f857f51f406b19f881fa694313513b35a8fda2f63427063bda6e728e5783d8d3ceca853e8278a730c12fb740a52cff" },
                { "fy-NL", "6c48c31583964ddb6378faa900e593fec3b3f5730f0704ee2b1bc0023aa9e124ee5f7fca2e47f5aa2d71916988336495c9c6e28022504c8eb80c92f82676ddae" },
                { "ga-IE", "d7172b3b37a1db585562f24c0c4ee9522116293cad3b4ecac727dfd9a8bc1340713fadb48801dd553e432f490ecb10e6fb9c8472767486611a1f857adcaeb3fd" },
                { "gd", "6c0f5aec8eba3c141bb86119e55b2ff46f3b23ec8340efc18ccdde6d139f6616bdfbed416bfe004f97139527e0735e5790fa06f2f35ca0dea7c8dcdfc31080c6" },
                { "gl", "e6d5a55b700648c22e6fdb2cc6a24c24feebd7d8a9b7defcbb1da0cba978e9bd1be883ef5fad8eec49e6639ddcdfefadc57ca7a6403f662c21c590765fbba6c4" },
                { "gn", "7873ba6324395157fd3bb293d093fc48317e1fb702d2ce1c6e783c033acb745486a687b63bee54577f1276bdfddfe74779cd29a99aecc227469388fe27491e1d" },
                { "gu-IN", "c49aa3bab435fc4343051fc6839f51f6c20d86d3f71d1368a3620b07b86a0c083b66a95b90c93cfb31e3a9ca60045e75329fe0ba6224bd9b30633ea4809deaa2" },
                { "he", "6bec8027c51f4eb2a2c9f979557b746075a82a799aa3b11282b8f73f10b12d6e34dcaf8a8e8784242e8e3cf77000283ba3a0c28633bd70b05b74e6911e52cbf0" },
                { "hi-IN", "73f1b30795f42b852424205bd2fb2c2dd8f348bb35f3812450319e57ea269b297761896462cb790dabfd1f14c5b71503c01a3470930a658e5a9bc8deed6d728b" },
                { "hr", "321029bb75627aa0208fcd7c6aac547da4b4f80cc6f200de878806c67d1f41b96150e2d3a3bb7ab813c91e0950df84492fb6ddeb98e3a82aa684fa92bf04c515" },
                { "hsb", "bffa60cdd0c8008d18d6c1c2d82b736397d37b2bdc6c6466520b65a2659bf84d1135e1c9d9e3bfab070ea76f9675e1e5d111667618eab637de79427346b90116" },
                { "hu", "71a3f991f63a98b70c1e5a341ea977dbc270912114696dc553724419da8a346e5387474832471de831ae18a2dcefadd0d0586a9cde422a2f188143688511ba8c" },
                { "hy-AM", "6b765c96d8069d22f75e03fbf30d7236585d137bc4c1c17912ff821f26a1fdd10030cb9db527b58adeb336b39663a427a1058365be3622acb8b6bb1a6832730d" },
                { "ia", "4aa4b33b7f0dec8f0f121c3987b20f40392ba4f58ba5f46a9cb9cbd3376cc8f6dad7aee78fe1130c89597f5196fd13acf1a44961e1081195aa416155ca47e61c" },
                { "id", "a84ae76693a5b3e515849a381a81b8bfe8c55d79a5b0a480a5ef0195089e2657dd8ca22cacce50e432454f5cc6f2da085de08f8e7c1245f08fc549a844f28574" },
                { "is", "c6562846460c4fb2eaf17ac73d55190fb2b87c1d60ac122bac1d69013a302362577d895b2be86d0701df130761590f90743bb8aaeaf04481e14634c9acfb7bb0" },
                { "it", "dd64b3dd3a5d503bb913d50751ed01f89896236d1c3e6e923d9ad89fe7a18b8aef3401ebc9245f14b52c461b2c9c8c14a8f9fdd6c1d1018edc503a2ad05f43fe" },
                { "ja", "1d5a7f40a5fb122274cdb3cf2361e561c8a8c77623e1686ff6baae3e0cfff69026401fc08fe691528acac30113ded6210d8f00725e90d059c99dcb7dfdb6fd33" },
                { "ka", "203d19d8b973fc65ca686a5387231f80ef80a431b8c7d1908a2fbf988926804ff495e012982ffb3a9bf40f4e80ebf3878a674777c0b215b42daff5e50d8eac03" },
                { "kab", "1425683116dd3f20e1e4821168862693ed308ebaa940a73b385387e6b4584122bbdaf9f103d3aa37ece999a70b434aa446d725270df28ec0d34fa613ca92bb34" },
                { "kk", "cd8bba32f9fa03ceb604268249778cd4c001f33837debda9ab97d0e25be0a73c1459400d4bdbba4db6cd47ea0f6d96a49fddaecf91056051774b598ef6220029" },
                { "km", "c37b90febc9d496d94845f3e2cbd9fa8e9101337af9f035ee7542d8f0781f448bb854ea5d03691284e06a2d9505f986f3ab5eb2d283fb3aedd83c7f5b74dcf25" },
                { "kn", "eda8a53de546e263ce51b1011b37a6e81f00d113cc1383ba977d217f45f1e3e62ebe47b76c66b5914f22659074a60ebe70f2d2b1fb4140f9c40a9fe4b4229bf6" },
                { "ko", "4257eb0c269910fb86655fd51cff004296f1c7e855a2544c20e6bd8f781099b661f8d2de79e30fa8a48cdb5014b2826055a7cd1c87b7e149b307cfc37196d184" },
                { "lij", "d247336a48fad3fb7826345fab69d2196959a4e24793881cda67496c8b7e1b2e636670985b4b0f8cc6742dbdab73ea2714f6ee2fe9b33924ad268f7695663a95" },
                { "lt", "4ff8f780587da98c2cb23de8c2e678e0f9df89a5fc7a794d199cd129474c08652d1cbcf8576aec0498bd5bc25a680034b55b89ee7902e690e4f06c260c4ae07e" },
                { "lv", "fd87223eaabdeeb310d592552492fe82f36cbe9798378c9e63d23d74df8a4986152a472af36c90bec66c85f4225c9b83e5aa23cc39cef5b33f52a4bfa9d3578b" },
                { "mk", "bbd4205987a4d17d73ba65141b221c0e4388f8417095aa2427cee1faa6084a70634aa0b8ff50f61cac11ea8000614f0455da5d64ceaa150e7b59641a8fe11325" },
                { "mr", "6465a13574b05aae0d48f8701ea04f0cab622ec00d242815186b321bc9ef09fbfe0c74c3f7c457874d57c1ae7d8fc9a374e139b8d11fb41e55009d9b411a4341" },
                { "ms", "f7cecc927e6c8bd3c1fe2174e45f9c997abba13bedb3816d03df191827db22566a8e60080ba5b741ef8e4fd9a01d8e1b2769412ed9e69a99615bdeee7a83983b" },
                { "my", "39ee326a55be11b274101e1de31ec00db4e2ff1c06d285f7f35458fe50dbecaa6a157359979acacfc614c92272d974e27a9576413e712eaf994addedc92a7568" },
                { "nb-NO", "ff22633722ae054b9f82d20ac0f77ae9e1f5af64506bf46a49175a1ff4d215391201c9ad604d774a9740d4e2b3ddbe7d00676949c269e7f7b5269855161c61fc" },
                { "ne-NP", "15dfb47ebef0ae8db5a27681cd62b3ab38040828343c263bc490dc379a0c7e8162fb61b1aaf81e65ee64ed82b5420a00034730f95ae9dac7c5792db97ab1a184" },
                { "nl", "3dbf8b78fcc0033ac3c5ed3cc0dec7d00f13fddce17a0754c866a0d5db8501edd5ee253a7edd9b9e08a90a158e15518142732e58bcab52f2ced9d058378de4e3" },
                { "nn-NO", "be49f041fb8f9ee384fb68dbd55357063abf99ac5f72080af3cbfd46513aabcefc56d6b9cb59abf3ac26bede5917ee4e13206d1891bad45d97439da40d3733d7" },
                { "oc", "f3cfec1c20f1cd959b9fb8ee516121b917c15c9c4b0d2e8b9e6a6bc0335a2e7fda531504ee590d1681c48e50d2ab2586d0fe8f35166511ff827e47d3cb0f2d70" },
                { "pa-IN", "e8690831bd7749a7fabe52a23aaff697d2b5aea06e2ab6282c61fb24d82ef46ecdd2e854bfb8327babeed96408057dab5bd3da86299153599f95a8cb913dd56d" },
                { "pl", "e7f1948f474a2955b09ba84679d978abbf2d534ebd4cce6bae9acbd8cd0dd06f2b08294487086e92ab26de081ba052dcd7d3e17aa90db32ac91600d0044e6e8a" },
                { "pt-BR", "b5898af70ec76211ce4a8f2ca0b6da2de42390be230f51b552ececa73e61c7d14953e7c2d8db6c52ad08182ad3d449a8b4e7e21104de693adcf1f7b91172757a" },
                { "pt-PT", "e68aefcfb6e8d2c210a002b2ae146c17eef7b47da2c856c598eef1d3e317fd8462c257df2e23bf9b48fc3d1cea980cfeb601bdbb10f3194808fc01ecb29aa9e6" },
                { "rm", "fcfa2fa2fcf8886d9c287064bb0ca4ad765607841fc55392a82daf92b3ad202b61334f97870ee31c1f3f919de919a0df453d7ba6dc16418968bfcf9cb484d9cf" },
                { "ro", "db0b12f6bc702ea07d664ca91c98ca5a68a1410c2e9dea3c278180a40f6f1a7526b033e539f10544c249e72bc8a8524203fc051c3f4ee8829a36adfe94dd11da" },
                { "ru", "709226c3bba80f950fc923e4a8ce2ff623df1de10995cf4df2924d3808e63d3010ec500c092a435232f9fa312d4773ebc34dbfa4181c17edf2dcb5d6705218f0" },
                { "sat", "51c9c3653e25232a525b58a03a3be18699aa29e2110f3006548a61d8b8c256d5907353f8cb2ba91ac5423ca9f3734e118989aef549a078b2668deaeb67c17fc1" },
                { "sc", "4de0870be397369957f4ae2a24ca0ba2f6d340b573db642a411ac8c30158681509c336bc7f9bd3eeecc57addc3cc7c132abb37cadce8d7c4ac20d678670e6445" },
                { "sco", "085f04fee612b898282778c570adb42e5f179f5903f5ad8fa0b8248bb63e8deb5657fdce2d85646c331dd89ef93fcc5f47a9fc49b38c2dc9d7ba5de9e3edc6ef" },
                { "si", "afab55772647554f91fe65058fc69d922f2d70bac3407c3c8586d56bc559f91daa71556a9f77f252c5fe360a5bd8b05e2b697f469ff7ea4fd6c1329420f7611e" },
                { "sk", "830779475c2497ff98a4f0e28c335f584da0dd3373e08aaadea99aa74315f4d55469441bb2944fb739f951ca3b3dfa3f9ba4feb0767718b61e2b8e220e3b6b2a" },
                { "skr", "e1d051ae8a42fc21517011d9827b9f737f9d307a65e0b4d9734329df6b1cbce091a0f98753b3307566eacf8f197977a19427a03db7e628aa64d3847670fb353b" },
                { "sl", "65b2898ad8437c0674a34dad5b3f0148fd431b76d84b40cf441c377623235f6d81de94980f470d887bc899d77046ddfd42a35681347275d54e376bd930848bb4" },
                { "son", "1e7601cdb9cc212eea06d80a7eee27ec14b36b932b2229e421646a96398182fe382d74bad29fa1b22ef43c2493be0b42091ed6109e2b0dfbe709f3180ce3e248" },
                { "sq", "142518929321c1687a6abdcd6ab8c30974b0126b0139a1daec66f0bdd43a5eb6de9b64681ee5266e61ea67ce6ef11f81c5d792dd811c5d71a9621fd6f3f0e2e1" },
                { "sr", "91506bcf3799afb221b66f038c7f932c69ba3ad300550fcc25da1d592f0fcd5ef1a76ef5be6e9a48ca4b31b2967c77010595c1ddf276165ea9fa92694237d447" },
                { "sv-SE", "22429f7859738b00544cfdd69d2adf11ef4634ea6116d776a44e32d7f522fa032407cd53e23e3d029be8b622fc46220460e73dd6a166350cc621ba31ad7a4164" },
                { "szl", "08471f3b44cc52a611d28e21ac6d81510d2b1e42bf2173e90c8693146e2304a8061114c17f6b97caa4146a32adc4861af3131eee8aa63784d8dc4be4997e63d9" },
                { "ta", "7fceacff3eea58b0b939426a3e65be0ca4c5174b6987dd18eb878c52a3585b36024ed88ec72e64c02322e80130c7c77e451bac7fa0b7e3aa5a87849b45339d16" },
                { "te", "16842a2423a4fcd03fdfcd8d71d9ea8ac5ff1b2197cf632e7fc5ddfd13d1d9d6629d7b5b3a51c133733d833e83a9da256dcb2069e30e68788a5cb0ca69cf4d2c" },
                { "tg", "27e1c32c39348a5a5b78873d6ad67986e3227ab45050cd03578f9cb7171972d0100821b67c8ce877afda25e3b60d0b074af9b67dd4b6a46ad376226244b2b80b" },
                { "th", "294029a50f131058b34f22430e978fb4ee29141225c0023c5582907bc9d9eaf7668aca09c9776f88a8911328eb80a4bfed8225b4611489f87d8a5f129ae3b20f" },
                { "tl", "bba01139537d34f14c148e26fc54e670059c8f87b79587aae33e67ed8314f5f1e615ba526a5967471a1aa59550737f11c0104b7dcc594fd28d692012f1af96b3" },
                { "tr", "e991e7283d9ced136bea1b17701557d90946e7f63b5e1647363ad3260c46c9c31ee0eb3e54d021564edb6684ca69a9ae4cc8f0e5b652d8af116105f4c7077f87" },
                { "trs", "edeeaa00a23b6ac327b4602182b054dff05a304dc65123ea98f7c0edc96477c4191ef50ecbff25363750a4846241caf4f793f28a0231a0b980635fc1e4a94130" },
                { "uk", "ca5192924959dd2c0e6816cc988cb5ad459c8bca084a72d597312c2f97269794060ff1f0234a53a3215156d902d9fa5154612ddd65f1f241b9a0150ec76d7585" },
                { "ur", "ee390dd383137b912f22f6cf9e9722fbfe4373dc0aba757619b91dd138efb87dd91f5eaa9a6d7d1829a3bf47bfbc0502a89c06158118cc080cbc6f56e852d9a3" },
                { "uz", "76802b0e93137f22293a657f6e160538a458ebbc98f83be8140955d5258fc629a789f9ba346f88aab2b797a533973e0dc398855e44c541d1b9454c014750b7d8" },
                { "vi", "76c73ede6aca7a68a26b8414e8f004eeb4e2a514292e98bfe0eb625dbd819b994e2e51988268d6849f5c2f4d6fd523ab844929b7bbd641441e12a50e9ce20fa0" },
                { "xh", "414eb545b24776944ba0f035d0e7e52b760103d053841d8a58bfd0a2a084510ce94e7cdf20d59146d85777952a26ce68499915d5bcbd197a32fa64cf0d4d0f28" },
                { "zh-CN", "70bf8f6dec86eb4505b715bbced8bce90ec5ec568d454650f75ca91f9ea30c76974ab30a98c3fea9ad3129f9b91b49777398bbb895834bbaa3b20b74bd73a598" },
                { "zh-TW", "cadf95107a7926c3ccf6bc58ba2380d54b3f1fe8349bc81ad098a8ae6bca70f71e57b5dd2374867331c16792fb1a9bdc29ec97486781d9156dc8aa1948b141b8" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/137.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "cbd64062f3021bb9ae8d440c4b21ecdfc75650fb1b7b2ff7d89793dccb7ad83fe8fb7c15d383d5073dc1fdf8f8265a9d6f18985bbe014bdc7afdfda92c969457" },
                { "af", "94956e077151b9639710aa62be560edd7c4dad1b5a82479d1f057aefd55294eb52229ec33391bdf5c782526e53d7d1fc98faa63bf58b1c7b59fd8eb7ce0952b8" },
                { "an", "2485edac3feee6e691b2b126d89de26f166976dfc8bdbad814217741b24b42349c574b9bec700c49c245e86a2a2bca1e4f85ade90abe108ad39223fbd53c0769" },
                { "ar", "33829f86a3f39ecb02561757364471952a882e1f32827dd0ead704343d88bf00194bad546b7ed8eebdbbb9a684f1b12aecd7c36f360c9ab30d499b894569c66f" },
                { "ast", "6054c5d6ed1ea10b80d8317965323b7c98542e666969d7ff4bbb640fa03aa38b83cd69c0222e2eb3539917bd1e2847ed762ec168e103a2fe674a8e215004d047" },
                { "az", "0451a557942c28934271683edbada2a6a0d06e59e2f00529aca9c30dfd2306d80f72e8476813dcb405f75dbdb9fa9093612332f705eeb445dfcb260edf38d173" },
                { "be", "dbad71474c3e39ea9b7e624d804ded9f4af13a6fdd2446ceb51488efb3fa781f685b6813e1254520b9813b1134a6535445b907aebaea56d7c5fad057984422c2" },
                { "bg", "316f7df9f29302c29760b61f1d042b1b4cd34d8b79b2dba2d921150d32ee7527b050b6a03c5c9c10c3f6400ad6f60cc3b98d374e2f38a3b9612ef8f3134b8f92" },
                { "bn", "931d5aae9388ee0f0dacdc24478ea2cff9c7d86862da5d7b5ea024d6cd35a489715d94835a0cb02935cb396b64ec734196086cbea3500b8d6c0b0c9ef6ce50a1" },
                { "br", "096b89bed4a7c2b22a9493f22e76d27c1d82de84e07f5cfa03e6108051b05fd4238716b9d0e11e74ddc836f587868b301a3a5723519d99512c9b99f639837d44" },
                { "bs", "66a12ca3c125d0a27f664328ab4824450ec94e6ea416b7191697bcde183e4ba98c39e83e40a06ad0e2e213fb0a469bc0545df8d3daed200f5a7503070ff82db1" },
                { "ca", "da13e6c801c529410f21c908eb31ff094be68044b4c32218ef1fa0454955e555b6887210d10b25303b4998a30c6244b0c860a44531fcf0f1f63bc3e1627263c2" },
                { "cak", "3aa8610528908d89ddf1eadbe7c02e0f5f6e152b961aa25e8a3a54b2b68de1aaf3aea7e7963806aebb776158aa7711b039c440e84a25fe9dc19c00e2e2771138" },
                { "cs", "cb9657857b6194066230c44d9725c8bb67e8c8b8bf519971e696a952efd63eaa3701f91a96386f86818112b1e0ee2166d93c92d88b563e82964f3b4cbe00b2c1" },
                { "cy", "374d15f8617f55f4131b7ebddbf4d8080e3a938f23fcb267538544516358e4899722c06f51166ce6fede0d50d904eae4fbd21f0918289b5441908efff78185ef" },
                { "da", "3de2ac21188ad5d106b67427d3b396f50def141aaf3297e11f7c62c6a3d571c9fc079578d3ec5bad7cf597a92521bdf21897ff1a0c31efe920ba9edb7cc91ab0" },
                { "de", "ecd6d4deb4dd39f22c5475fa586f1d81be4c26d374ff27ffaa9040bbbbb287c603f06fa8a442492450dc48f0157396b262a15ad9759ba6aa602da41b4ad7464b" },
                { "dsb", "7712354489377f23a6cc2bc77a17722e93f89e53810ecccd1a53cd1dcadbe78668d69ed37bb5edb17e47e5d16fa848ffabb359e23ce7f4c645e5d7f3761cad5c" },
                { "el", "749011e7cb31a391bc04a4b76fe8b17b92ff4bc1c8457a126c247feeda10259dbd9d121c34b3c1997ef8d0b50c783783dbeb783928e6fea11ab0e05bb8438e5e" },
                { "en-CA", "f81307b7c312715bfe23e52fb582cf94b59ec2878b93a42e010c2892a5b879523d3fee3dc109071f3a1b2bb164fd0d360f078bd0c5a2450d1fa05d9be74f61de" },
                { "en-GB", "474a221fa85e9d9e421567b18ef1a8621f006de419bc6a00ff4da73da1d1aadcc725e822984eb386d8fe6e7304883a2a415254857ec9297127ff7f1e4198f474" },
                { "en-US", "37b36c86bb85048836810c5d9ee207a86868b0583338b91e74552250f121123a355c1bc79f4eab04e390fd8aad47169dcc77ca504819a64d520b1434df431279" },
                { "eo", "c492a6b29e3d2fb1a06a2f5b3ffdbeaa84f1e82c112d7d05e44e726fb2d29be029fd933d462a694a097e41b98880eae37f6d01d50c660de2841b968139c8c70a" },
                { "es-AR", "ca78a8d3659b77116a39723f1364fe29c0d8da6e329eaab4cf65efc258ba1e01bb012ac981049718adcb4b1acddcb5decea50039eee6cb1c5887c644a3538bcd" },
                { "es-CL", "66f82b45f311eb62c1d973715558c5bbdc79627eff69cea14e635dba4de769277d52884a66795bc9cb81cedf4f896b8dc425fff11d7e9dd5325087b03549ae09" },
                { "es-ES", "2dc5d753c4c7136f2e589000631bdcfe8beb9815b246ea4cd3a62729ebf4c1e1f1d6cd08d2994661750427e6c786d9fe6f116d9c5cc25b2d358a698cccc8fd53" },
                { "es-MX", "383f3ea24e7c337877a07bd1246ee486734a1b2772eb8509a7cb40f4bbe808309d85ef16e7b6837976a258db655f588ea0fbdd3f9e783c36dc379772718bfc69" },
                { "et", "021d10c893a174c28c28883338bdcda42e00cf7d1aff59d7b0396ff8d2e24b3fa1ac4e073ef5367909069cd7a6ab8bc228bf5b464b6dd8bf0d774c7516d14eff" },
                { "eu", "4485709a0a57fdd525a9e4088a9b479a5bd6c77597ab556a75a0970c8831da28aada5b5ce2ea35e68d071548d5095f02baf69e9ce87828d06a0614e8c1de0620" },
                { "fa", "8f998794fc75636a2632911d964bd32e5eebab1aa9c8b91df3cdaa8b3588daf74543873e7c562cab56f6927a9ac17cccaf7f94a90a4552ed0652e4e117201050" },
                { "ff", "a43b358e1615bb0e622329ecd9e408ca82856b11224a81432545a9fc05cac08ac9daaf6a97a03b5078f91605180eed33f5834760462892e2f6f8eac478946a2a" },
                { "fi", "cd04c5172474fb70a53190481f4a59d77f303a11ca0872e3603bf3612eb5e107829d321eda74b838905ea8d1372fd0a28925c09c6eaea366b0f07c7ed51f5e17" },
                { "fr", "0172f108e2f57b311e8eb6e9a6c91053df2aed85c638263845b8a00a9f50d8dc1d6e79b966227c57d4d93ed43c5feadcf86aadce92e955ff88905d54ef8cb9db" },
                { "fur", "d29ba0df1cd6ab2d6e1470001681eb2fecfae5ce520ff6b3d41264caa14b9885f6a4f348287385eda3b28c7998b82ba7b75e495a0e6970683028a9abed0375ad" },
                { "fy-NL", "2e3fd8c5192f6f051ffda6030c9502dee7e9b375401f701a5d8e428edad4dae3c0eda00631c08f27035062de4e55776ea04ec77076debce4f00359f112547d82" },
                { "ga-IE", "b15c78eb31af28c4ce8250aef186bc5f507fc9dcfd7996a22c2e61cbf1a3574543ee37cbf7ed7340a2d53ec160ac0f74302281fc1d53b40cc1f208512f8d9456" },
                { "gd", "699daeefe3f0ce6a3cb680b4ede12934112adca64ad8b28914064bc85d66ef9e65cb39a69fd471231067ab46c64b6379d238aef25733ba2c8739fda3d9b62af3" },
                { "gl", "a47cc813434358522e4eab7cee6dc0594a126b0f45aa0834fc32fe9c4ac9b74e8a6f1cfcbe4fe9981acd45d6df18da047723021ecafda2f1dc8018b4578b9e1f" },
                { "gn", "0932a05c3e2efcc63f28b43eeefde6831f2d5e0b7184425fd846ba1685a3c056e5755e81b3da004e2e926971185ff780b2c6183ea9be1da9765906aee77ec7ee" },
                { "gu-IN", "81341ab544593457edd84e205529ddd8c4ae4e58546264506dc01f36c573c71c141d9b501b63bd69ea59c1712bda316aed571fb6f361d78f31547739265959e5" },
                { "he", "76819b6ec8c72b9edfd67a2fccf2a9882b641fdc23432420e24d31729544789a57262f01cd71184b547426f963eaf7c0df7e8d0ccbbd5c09822e1a5e25642347" },
                { "hi-IN", "67d8293428957df6d89b55501f9c1bb8d2d0d030d853790d052a8220702f670eebcf0ba1e3181dc98b473c2752b44b520fe303a267403a4651998399c07e5dc7" },
                { "hr", "d2309316f6d590469708c3dac6e929dee5c052896a18e2fbb10964e637491fd3fbe49eea272c1cc19649e47fe1e38817fb9e0651975ead1e96a459359de268bc" },
                { "hsb", "28ca573fcbdcfafae533f9ef350f6b97b633dabc345ad12d4e817376e8ab855a0f4464e6b9bf874fa39c5f9ee5cba33d71235980871348a062323502605e2e86" },
                { "hu", "9c4c7e82770068989a44bfc40db99f23617e4dffaa8d0f7c65bcee3035de926b2f30ceb4be06f470925afdcb84181d6cb934faf6d19e995b3b3ab86b6b224f1a" },
                { "hy-AM", "5c1c186d45833fa8fc159dd2b49d486f59e395c1d412af5805b92031a201e84622a80ff629629bb069bdccf5fe4b04a92e6e221e3f43809f22373533add38993" },
                { "ia", "68020d66cf0a1a05c9d61904ef189f9424f5cb142a4fc21bab0c9172c8928fc248a68da193a1a5dc4d0e360db78fb3213fd9c1305dbd7375dc1bb7b369cd5c11" },
                { "id", "0ed91528c35216ed8b69d52282e7b04d1cc9d2528c5f5124742ce47bf14213443c1c5d2ccf2713d3bb362dbd2bc63b0ef658202b233f8cd13d550219a58d34b3" },
                { "is", "e3dd9a1bb7a03fbb6b561972d73ea148d58093747243d88185dd90157a283da919a01ccd5cb487a01ce17da38e747b7a951ccf6e29eef8c031612e454ba7b102" },
                { "it", "abbde7286cd3e9adb16b64d7a4a937ab2ab9c5f185aed22700798ef2aeb340d8bf2520ab095bde1168970eb7c1aab28b156fb6d42a8f138ce11a59868e501978" },
                { "ja", "6c3a35ac939c5fe4d1ac0566e1a9d4f2618bc8831aaa7e50ca8afbe7c4d5a2b839db806263065f6b3cdf390f0c35ddcb7b93588e417305e9e0dd9ef61ace0ee1" },
                { "ka", "c86b96d5d9a1ec8ebb3ca2901bb8d0ab547df48abb1b4a15961c1d8999818d82c7d47095c003547946851e10c3f8d05564b36530f1ba1b953e5e0314ebd3dfd4" },
                { "kab", "28e8b16e7da021c6ab117b48dd75a52105bcf6f4e14cdbb1299ca33f4fd237b5566402ae351fe66a2b2d207d63dff8b9d4064b3fde3a77a2aaabe7fd1f9cf93f" },
                { "kk", "84d3214981c69593cb8f12d594994db788568ba5e0102109c047db5501d0fd02dbb7b0a7c6c73aba2c8d9eb9386cbb942d55abda821e3b26aca8a0a8a9094413" },
                { "km", "bc26981fb9ba8875a0225879ccedd6d3cb76c6e2e60a30d6cf97ab7e985223f97b16e5eebb2b708862e87427894fe4da32b9f0c9b8624b9adea1264896021ac9" },
                { "kn", "fee671a9a187b83b916c2ecf59619a38c2622a9d2d2219f1dcdef48fc9db1fe7970b5a0c05854567b16c3fe112940d3f0803c4929aebe6403b63b7be2ccfb7c4" },
                { "ko", "dcb32e979b906201392436689af2fe920305990a2c26174072e975950c2df29ba0dc6e8ff483baea735947ef1f8693a9cbce32db9de5bd340b2b59e748d7c332" },
                { "lij", "fe34275523db3ee91a01af15fe9b9035097d00b257a67c90e2113f11b8e394af3cb582d8fcca8945dcc87bf95ba963a5b9cd0ae29b99688d4d195e29311fc0ba" },
                { "lt", "81344f5390bdcd07080f2c1087eb4826be9ba89adeae3be1db7da0857c9e4c5f89ee52fdef2befc4a8ad3edc63dd3daa23e73d63608685840e8851e34611d21e" },
                { "lv", "edfeedb8bc78a0c7394efafa7d4996f354b9d8eb12bb3c36c3905a9d1d1b5c6b8040726c4354b126c4bba9f289fc2eb43a822204148fda643d789b4bd2575cae" },
                { "mk", "e37eb441c7269d1da2efe799f24279e6a81c9317191dd85603fb48d98ede642e8607c366608be4d37409ac308d1b02a587af49b4b3a4c25e6b737a87fb3182ff" },
                { "mr", "c16403867a6b457b42bcb9951a12bc88bd9c817105ffa2d9716c71dd324fbcf6a4c808e66bdff9dd300f01bcb14019ec2989ffe8def04e4872b4a078135e6520" },
                { "ms", "4b8f6c5c852c7b9e94f09c48117b7cf99e257c300fcf81e635bd557160e505ab97ffa11c357b82a3a5736a7d0a25e8349f98ca96f98036859f7b5c41ff6d7b29" },
                { "my", "ba731edcf0438822f134366c76a54d740414f3014d47f5418d218c3f8b7b3aec27f4b8dc76ef6697b984498adf3505c7b5b2e177cfeb22653629981a58a38039" },
                { "nb-NO", "eec212bc7ab1b10cb62d9cd127a2e39d688b4840ccc38a2fb96d3346aecb449ad7a3424cb8e3b69a6d9d2186ed465e085f225617b008c6441acb83644e94f7f1" },
                { "ne-NP", "403f8aefbf9efec1f3dbb05a3a393954914fb3f5c8ca4e3fb029d1f55e7b7380dc2dc853fe8a4f574fb0d9c25fd8000de593e2d430842d706d633fcb2595db3c" },
                { "nl", "1ba0d84e5d8df9b1be2131c2f0ed67944a9b05fb44399b6316bd75deb262452be7bc3e3ee5dda62446ae842befcab5c577b4290cd1953225ab2ead354dbdd1f5" },
                { "nn-NO", "021881d99d9242147224b075e9e8b84c678d20e3514fa97a43f89230f33e9432a0811b5692670d63ad25cc5befb44c5e47914f308dd4ca2d38e73934662e5f6e" },
                { "oc", "48173e242623d0e438e6377ac10e66fc2987d8000049ce3d40a145cfc57b90f8288cd465357eaf19f0a4e195382e010771e4a858f9e52ce4e124bcb5459e73b1" },
                { "pa-IN", "292af9da12501c0e0a1752573fe83c885ad5c0f145b08a4c114d7542739498bc9ab5d14ee62171483f78d65a1c4388f6dbe55bed352bcbaa76390f4f0d163c68" },
                { "pl", "5e81131af950d9a6f6f1382f982d0bac06bb665f1c2d30c109f65cefa96d5f65f2fdce658124226010c3c6894486cd10b75575ec5c32c6ba7afce14ab87381fa" },
                { "pt-BR", "4de07b884df50e7e662eeb6a732b1ae495e7b39fd64bc78b3cfaa7ca59450e8d727d367b098488863f2340d2ec0b34abc28ccf65dc6ec9fb14f3fb9eaec3031e" },
                { "pt-PT", "1a4139fd98fd0054e594eb0acaa128d2f6327c42d0d5b748ee90393f479aea2ba35129fc30b85dc856f26656b1975f00f0d702728715f8b6fc10522a04f81e7b" },
                { "rm", "13adaf148cc400a1d09e8329b6484006828ae20309192d05a5b972c93f2b6dc072bdc6da257b550032902068368cf81edf6edb1e5797c25bc577b81395e55416" },
                { "ro", "5dfa2d0f66891405074e387dc8548f90fe231b3b56d09586e84aab48ab2b587d72e1761583e3a9edb4e6635994ed338fd50554dbc570db89f7bcb63f340f76cc" },
                { "ru", "36910083e376e12d74e7ce90ea54d4f38562c8aebf64a98a65a0b408a5ff79b224020f5057e8fc1d27ffd40ee4e1ee44df034f096c2f3c54db5bd99d6f420567" },
                { "sat", "725b1bfde6526c26f2a8762d118dec826afe66d44d1c0b51806101be228cc499094b1e0ff1710e5cdf8c5636b011712923dadc8060b83772ef4ff9cdedcd8d03" },
                { "sc", "fd97f366c0201cec6998c882ba528a0cfc1ff1d1d15fdef33a661d0a630b6f86d269bf8ec6449d35303d9f13500c4f880c5c3b7c352b01f593ab9c5d4a4974ae" },
                { "sco", "0addc9734c4d1fec6acf5e9c00e058fcc094ec51d59fcc571840f9253d254e50b0c2b2226fa42ff370fa2cfd64219ece53452d360b5793a7c4409f7a3d07b535" },
                { "si", "dbf31331b01cfe0cf3ea99ce32cf2accc6b14ba52757547d106c25db1732cd3ca866eae63f8e065a7e6cc20bedd919fcaef81fad4baf7175d810583120f3fd03" },
                { "sk", "f3e73cd3978408579d206f0032a18990bd45a0163da1fead87ecde7f8b5c11e745481df6bd2d3650a0ef58642faf1eb5fc7e1465f85b5670ec0bc292e5541d4e" },
                { "skr", "614d8ae8605703d7722d1b9ae56bf25a3d8f8c5c7f43f7de3d08c8e86048eddf8b7714c572e38af58ce47dd3aabebcf69db3b4071bfa9c55b31f84d37389ff56" },
                { "sl", "7a419cebd9ca3df28cd4b7d2fb0d412b98760448ded8086a4b87bda06255b776a4ecc0741ad5714f56a0b1cdf7296af7e5dbb7579c74b56f84af7b17759cd5a3" },
                { "son", "3f1ffad186ee8ff313c1b13e3da881fa6c7fb2047ca72631a928e235a903f85728423896a4aa43a10c931a6d3a8ccefbf7f6a2256ccbfbdf324c6c29ad32e3d2" },
                { "sq", "c15e254df81d64f0ba6b727ef63e99e4005f88f36a97e9352738a46fb56031989fd250f47a8e545555fce1ca433a9c56ed6d954235d3b4e6b8781dddc1b552ad" },
                { "sr", "bafd34607b227442d7e84261c0e7fa8bbd9860ee49dd04993473da25508a9a9f2d58ed8b85008a96616f4d460cc5c14dbbf5308c58e72a6eb45958d2b45e0901" },
                { "sv-SE", "bd077f21d24ff038a782bb3b4416bf7dea182622c8d99d50982dc6bbc8e182b54f453b9f851794372ccda008ab3fe019ab01d80e889fbf442baf79d7b6b1362d" },
                { "szl", "75e6399fcb8b88db37011adf9b97cf05e29d4a0b658c11be6b608390ae3ec4e938e74fc4abbf8d9293b8439b0079ee6889a41703124757457899f031e5a16f01" },
                { "ta", "6dd296a6537cef0010eed69229bbb02541a8274e476e025418060ffbfdb88c91d329bc5b661948234eba401cc31e637dfcac056e0cbb979b0bba4b6659e802fc" },
                { "te", "2e5398792c3bb99c536adb8e281180e2440363fa6032482b2f38aeb803bf1be3f71bfefed41d57e1e041126ac4239fe1f935e25074784d7678872525b0d1ca4b" },
                { "tg", "cc5ba26984126cbf38f77f68bff0c45fd0a7b216addbb946a8246fdf5502741c327c97e21ab07ca4641f0fe3b26aab536f4c68b9edf3c0d498a845593a3b2fa4" },
                { "th", "c5c41ef9297921608f0ca746fe7cc0b6654edb0f4afdd222a508a87d92f3f31ddd12c04d573874b6419b30100cfe73f4d552570fbdc6fba2e22ae1d6a39ed5c7" },
                { "tl", "3d91646b58ec35cb558574c272f9f1c6088efb2e154cc59c8aeac9e05afbecada28431df65dd8e9530924d5698af549aee3f5753e27b95644240e9670ec134a2" },
                { "tr", "3d9ed5f8a1a24e70ac0792521e37e77194dc44fac8c95eb13901f03a5e49d38a26d812e6735f3b38e54174623377ca03635db4395fe4969db8f455e0d7c2169d" },
                { "trs", "29c617f758c5f69e44b15656786a10d0fb6b839af7730fcdcb920b30d0ad7d782d6478ba8ec10ae106b834e4b2aec746e40494d194779e74aacccc82751fb227" },
                { "uk", "2a8784c9a617cb092b8bac95641e9890be151e42bd778c5b0ba064aad4a88a76c4bb34f878ec1cb6aa593194a3b091c1716f7e9a04871e37cb4cc366762c26d8" },
                { "ur", "3a30fef9e8c646781a0bb40de257554c36332f1795d7ecf1833f67c4139caf74584b6f441e7026ea23e0882b1dce54213088a7655f63e3ae0e2adcad30f0693f" },
                { "uz", "35d31ce710b27895534b22f19b1d40830ea7287369213cf5c8b8309ae7627024c1133e787d871340dfeda56560a267dfa457d62e5408da2222189249cc62119b" },
                { "vi", "d2a1aa5dcf056fbef4f226b5be03bc07ac47a0c957b36363ab26f5b12fec23e1cba46e25b9e4074d66e0811b91916b8d42d8e738464ec7f00eeb4a3be701fa2d" },
                { "xh", "e957ddaed3817f2c8c65f2133b4c21efe0746ee66cc1c28b6fd19ec6dc965e979669f749aac368161b60f8bc7adb1724580504e8520063ae58364290962aee1b" },
                { "zh-CN", "9e20d7e9ea88ed644a457f8d64f4e53f80c227beb1c9d1cc1247f69e8c67e393634667c4c9760d42220630ca856115aeef3915b11ee6f3f5b3a59763ec789480" },
                { "zh-TW", "b9043eb4ead5ba76b460d451939fad7d9c5d9e2ef9a2c139d7e62f7220bb6b046928c07f8b5e43b715c7b78c75fd9a11649c25c2207abeca1130e0e642bbf8cb" }
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
            const string knownVersion = "137.0";
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            return [];
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
